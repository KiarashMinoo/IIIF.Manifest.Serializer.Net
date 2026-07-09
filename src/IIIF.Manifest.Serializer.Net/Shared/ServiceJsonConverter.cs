using System;
using IIIF.Manifests.Serializer;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    private const string ContextJName = "@context";

    // IBaseService carries this converter so element-typed collections (IReadOnlyCollection<IBaseService>) resolve it,
    // but that also makes every implementer inherit it. Reading/writing leaf types below must skip this converter for
    // the leaf type itself, or ToObject<TService>()/serializer.Serialize(writer, value) would re-enter this converter
    // (recursing forever on read, or silently writing null via ReferenceLoopHandling.Ignore on write).
    private static readonly JsonSerializer LeafSerializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        Formatting = TrackableObject.JsonSerializerSettings.Formatting,
        NullValueHandling = TrackableObject.JsonSerializerSettings.NullValueHandling,
        DefaultValueHandling = TrackableObject.JsonSerializerSettings.DefaultValueHandling,
        ReferenceLoopHandling = TrackableObject.JsonSerializerSettings.ReferenceLoopHandling,
        ContractResolver = new LeafContractResolver()
    });

    private sealed class LeafContractResolver : IIIFJsonContractResolver
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
        var typeValue = serviceToken.TryGetToken(TypeJName)?.ToString();
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
        }

        // @type was missing or unrecognized (common for Image/Auth services, whose constructors
        // in this SDK never set an explicit @type) - fall back to detecting by profile/context.
        var jProfile = serviceToken.TryGetToken(ProfileJName);
        if (jProfile != null)
        {
            var profileValue = jProfile.ToString();
            if (profileValue.Contains("auth", StringComparison.OrdinalIgnoreCase))
            {
                // @type alone can't tell Auth 1.0 from 2.0 apart when absent, and both classes'
                // JSON shapes overlap enough that the "wrong" one often deserializes without
                // throwing - so use the (always-set-by-this-SDK) @context as the primary signal.
                var contextValue = serviceToken.TryGetToken(ContextJName)?.ToString() ?? string.Empty;
                var preferAuth2 = contextValue.Contains("/auth/2/", StringComparison.OrdinalIgnoreCase)
                    || profileValue.Contains("/auth/2/", StringComparison.OrdinalIgnoreCase);

                Func<IBaseService?> primary = preferAuth2
                    ? () => serviceToken.ToObject<AuthService2>(LeafSerializer)
                    : () => serviceToken.ToObject<AuthService1>(LeafSerializer);
                Func<IBaseService?> secondary = preferAuth2
                    ? () => serviceToken.ToObject<AuthService1>(LeafSerializer)
                    : () => serviceToken.ToObject<AuthService2>(LeafSerializer);

                try
                {
                    return primary();
                }
                catch (JsonException)
                {
                    try
                    {
                        return secondary();
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

        // value's runtime type (e.g. Service) also inherits this converter via the IBaseService attribute, so
        // serializer.Serialize(writer, value) would re-enter this method. Use LeafSerializer to write the
        // concrete object's own properties instead of recursing back into this converter.
        LeafSerializer.Serialize(writer, value);
    }
}