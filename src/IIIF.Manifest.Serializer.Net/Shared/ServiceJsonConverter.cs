using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace IIIF.Manifests.Serializer.Shared;

/// <summary>
/// JSON converter for deserializing IIIF service objects.
/// Supports multiple service types including Image API, Auth API, Search API, Discovery API, and Content State API.
/// Automatically detects service type based on @type field or profile property.
/// </summary>
public class ServiceJsonConverter : JsonConverter<IBaseService>
{
    private const string TypeJName = "@type";
    private const string ProfileJName = "profile";

    // IBaseService carries this converter so element-typed collections (IReadOnlyCollection<IBaseService>) resolve it,
    // but that also makes every implementer inherit it. Deserializing leaf types below must skip this converter for
    // the leaf type itself, or ToObject<TService>() would re-enter ReadJson and recurse forever.
    private static readonly JsonSerializer LeafSerializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        ContractResolver = new LeafContractResolver()
    });

    private sealed class LeafContractResolver : DefaultContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            var converter = base.ResolveContractConverter(objectType);
            return converter is ServiceJsonConverter ? null : converter;
        }
    }

    /// <summary>
    /// Reads JSON and deserializes it into an appropriate service object.
    /// Handles both single service objects and arrays of services.
    /// For arrays, returns the first successfully deserialized service.
    /// </summary>
    /// <param name="reader">The JSON reader.</param>
    /// <param name="objectType">The type of object to deserialize.</param>
    /// <param name="existingValue">The existing value being replaced.</param>
    /// <param name="hasExistingValue">Whether an existing value is present.</param>
    /// <param name="serializer">The JSON serializer.</param>
    /// <returns>A deserialized service object, or null if no valid service is found.</returns>
    public override IBaseService? ReadJson(JsonReader reader, Type objectType, IBaseService? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        IBaseService? service = null;

        // Load the JSON token
        var jToken = JToken.Load(reader);

        if (jToken is JArray jArray)
        {
            // Handle array of services - take the first valid one
            foreach (var serviceToken in jArray)
            {
                var detectedService = DetectAndDeserializeService(serviceToken);
                if (detectedService != null)
                {
                    // For now, set the first service. In future, we might want to support multiple services
                    service = detectedService;
                    break;
                }
            }
        }
        else if (jToken is JObject jObject)
        {
            service = DetectAndDeserializeService(jObject);
        }

        return service;
    }

    /// <summary>
    /// Detects the service type from a JSON token and deserializes it into the appropriate service object.
    /// Detection strategy:
    /// 1. Check @type field for exact service type match
    /// 2. If @type is not recognized, check profile field for pattern matching
    /// 3. Fallback to ImageService if detection fails
    /// </summary>
    /// <param name="serviceToken">The JSON token representing a service.</param>
    /// <returns>A deserialized service object, or null if the service type cannot be determined.</returns>
    private IBaseService? DetectAndDeserializeService(JToken serviceToken)
    {
        // Check the @type field to determine service type
        var jType = serviceToken.TryGetToken(TypeJName);
        if (jType != null)
        {
            var typeValue = jType.ToString();
            switch (typeValue)
            {
                case "ImageService2":
                case "ImageService3":
                    return serviceToken.ToObject<Properties.Services.Service>(LeafSerializer);
                case "AuthCookieService1":
                case "AuthTokenService1":
                case "AuthLogoutService1":
                    return serviceToken.ToObject<AuthService1>(LeafSerializer);
                case "AuthProbeService2":
                case "AuthAccessService2":
                case "AuthAccessTokenService2":
                case "AuthLogoutService2":
                    return serviceToken.ToObject<AuthService2>(LeafSerializer);
                case "SearchService2":
                    return serviceToken.ToObject<SearchService>(LeafSerializer);
                case "AutoCompleteService2":
                    return serviceToken.ToObject<AutoCompleteService>(LeafSerializer);
                case "OrderedCollection":
                    return serviceToken.ToObject<DiscoveryService>(LeafSerializer);
                case "ContentStateService":
                    return serviceToken.ToObject<ContentStateService>(LeafSerializer);
                default:
                    // Try to detect by profile or other means
                    var jProfile = serviceToken.TryGetToken(ProfileJName);
                    if (jProfile != null)
                    {
                        var profileValue = jProfile.ToString();
                        if (profileValue.Contains("auth", StringComparison.OrdinalIgnoreCase))
                        {
                            // Try Auth services
                            try
                            {
                                return serviceToken.ToObject<AuthService1>(LeafSerializer);
                            }
                            catch (JsonException)
                            {
                                try
                                {
                                    return serviceToken.ToObject<AuthService2>(LeafSerializer);
                                }
                                catch (JsonException)
                                {
                                    // Neither auth service format worked, continue to fallback
                                }
                            }
                        }
                        else if (profileValue.Contains("search", StringComparison.OrdinalIgnoreCase))
                        {
                            return serviceToken.ToObject<SearchService>(LeafSerializer);
                        }
                        else if (profileValue.Contains("discovery", StringComparison.OrdinalIgnoreCase))
                        {
                            return serviceToken.ToObject<DiscoveryService>(LeafSerializer);
                        }
                        else if (profileValue.Contains("content-state", StringComparison.OrdinalIgnoreCase))
                        {
                            return serviceToken.ToObject<ContentStateService>(LeafSerializer);
                        }
                        else if (profileValue.Contains("image", StringComparison.OrdinalIgnoreCase))
                        {
                            return serviceToken.ToObject<Properties.Services.Service>(LeafSerializer);
                        }
                    }

                    break;
            }
        }

        // Fallback: try to deserialize as the most common service type (ImageService)
        try
        {
            return serviceToken.ToObject<Properties.Services.Service>(LeafSerializer);
        }
        catch (JsonException)
        {
            // If deserialization fails, return null (unknown service type)
            return null;
        }
    }

    /// <summary>
    /// Writes a service object to JSON.
    /// Delegates to the standard serializer to use each service type's own converter.
    /// </summary>
    /// <param name="writer">The JSON writer.</param>
    /// <param name="value">The service object to serialize.</param>
    /// <param name="serializer">The JSON serializer.</param>
    public override void WriteJson(JsonWriter writer, IBaseService? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        serializer.Serialize(writer, value);
    }
}