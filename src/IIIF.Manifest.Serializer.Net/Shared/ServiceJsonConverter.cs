using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Properties.Services.Auth2;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared;

/// <summary>
///     JSON converter for deserializing IIIF service objects.
///     Supports multiple service types including Image API, Auth API, Search API, Discovery API, and Content State API.
///     Automatically detects service type based on @type field or profile property.
/// </summary>
public class ServiceJsonConverter : JsonConverter<IBaseService>
{
    private const string TypeJName = "@type";
    private const string ProfileJName = "profile";

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

    /// <summary>
    ///     Reads JSON and deserializes it into an appropriate service object.
    ///     Handles both single service objects and arrays of services.
    ///     For arrays, returns the first successfully deserialized service.
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
        else if (jToken is JObject jObject) service = DetectAndDeserializeService(jObject);

        return service;
    }

    /// <summary>
    ///     Detects the service type from a JSON token and deserializes it into the appropriate service object.
    ///     Detection strategy:
    ///     1. Check @type field for exact service type match
    ///     2. If @type is not recognized, check profile field for pattern matching
    ///     3. Fallback to ImageService if detection fails
    /// </summary>
    /// <param name="serviceToken">The JSON token representing a service.</param>
    /// <returns>A deserialized service object, or null if the service type cannot be determined.</returns>
    private IBaseService? DetectAndDeserializeService(JToken serviceToken)
    {
        // Check @type (Auth/Image services, which still model @id/@type via BaseItem) or the
        // unprefixed "type" (Search/Discovery/Auth2/ContentState services, which postdate the
        // Presentation 3.0 "no @ prefix" convention and never had a prefixed form) to determine
        // service type. A V3 manifest's top-level "services" array always writes id/type
        // unprefixed regardless of which shape the leaf class models internally, so both keys
        // must be checked here, and the token normalized to whichever shape the leaf class needs
        // before ToObject() runs its constructor binding.
        var typeValue = serviceToken.TryGetToken(TypeJName)?.ToString() ?? serviceToken.TryGetToken("type")?.ToString();
        switch (typeValue)
        {
            case "ImageService2":
            case "ImageService3":
                return WithPrefixedIdType(serviceToken).ToObject<Properties.Services.Service>(LeafSerializer);
            case "AuthCookieService1":
            case "AuthTokenService1":
            case "AuthLogoutService1":
                return WithPrefixedIdType(serviceToken).ToObject<AuthService1>(LeafSerializer);
            case "AuthProbeService2":
                return WithUnprefixedIdType(serviceToken).ToObject<AuthProbeService2>(LeafSerializer);
            case "AuthAccessService2":
                return WithUnprefixedIdType(serviceToken).ToObject<AuthAccessService2>(LeafSerializer);
            case "AuthAccessTokenService2":
                return WithUnprefixedIdType(serviceToken).ToObject<AuthAccessTokenService2>(LeafSerializer);
            case "AuthLogoutService2":
                return WithUnprefixedIdType(serviceToken).ToObject<AuthLogoutService2>(LeafSerializer);
            case "SearchService2":
                return WithUnprefixedIdType(serviceToken).ToObject<SearchService>(LeafSerializer);
            case "AutoCompleteService2":
                return WithUnprefixedIdType(serviceToken).ToObject<AutoCompleteService>(LeafSerializer);
            case "OrderedCollection":
                return WithUnprefixedIdType(serviceToken).ToObject<DiscoveryService>(LeafSerializer);
            case "ContentStateService":
                return WithUnprefixedIdType(serviceToken).ToObject<ContentStateService>(LeafSerializer);
        }

        // @type was missing or unrecognized (common for Image/Auth 1.0 services, whose
        // constructors in this SDK never set an explicit @type - unlike Auth 2.0, whose 4 real
        // service types above always populate their own literal `type`, and so always resolve via
        // the switch and never reach this fallback in practice) - detect by profile/context.
        var jProfile = serviceToken.TryGetToken(ProfileJName);
        if (jProfile != null)
        {
            var profileValue = jProfile.ToString();
            if (profileValue.Contains("auth", StringComparison.OrdinalIgnoreCase))
                try
                {
                    return WithPrefixedIdType(serviceToken).ToObject<AuthService1>(LeafSerializer);
                }
                catch (JsonException)
                {
                    // Not a valid Auth 1.0 shape either - continue to fallback.
                }
            else if (profileValue.Contains("search", StringComparison.OrdinalIgnoreCase))
                return WithUnprefixedIdType(serviceToken).ToObject<SearchService>(LeafSerializer);
            else if (profileValue.Contains("discovery", StringComparison.OrdinalIgnoreCase))
                return WithUnprefixedIdType(serviceToken).ToObject<DiscoveryService>(LeafSerializer);
            else if (profileValue.Contains("content-state", StringComparison.OrdinalIgnoreCase))
                return WithUnprefixedIdType(serviceToken).ToObject<ContentStateService>(LeafSerializer);
            else if (profileValue.Contains("image", StringComparison.OrdinalIgnoreCase)) return WithPrefixedIdType(serviceToken).ToObject<Properties.Services.Service>(LeafSerializer);
        }

        // Fallback: try to deserialize as the most common service type (ImageService)
        try
        {
            return WithPrefixedIdType(serviceToken).ToObject<Properties.Services.Service>(LeafSerializer);
        }
        catch (JsonException)
        {
            // If deserialization fails, return null (unknown service type)
            return null;
        }
    }

    /// <summary>
    ///     Clones <paramref name="serviceToken" /> with its id/type keys normalized to the "@id"/"@type"
    ///     shape that BaseItem-derived leaf services (Image, Auth 1.0, Auth 2.0) bind their constructor
    ///     parameters against. A no-op when the token already uses that shape.
    /// </summary>
    private static JToken WithPrefixedIdType(JToken serviceToken)
    {
        if (serviceToken is not JObject { } obj || (obj["id"] is null && obj["type"] is null)) return serviceToken;

        var clone = (JObject)obj.DeepClone();
        RenameIfPresent(clone, "id", "@id");
        RenameIfPresent(clone, "type", "@type");
        return clone;
    }

    /// <summary>
    ///     Clones <paramref name="serviceToken" /> with its id/type keys normalized to the unprefixed
    ///     "id"/"type" shape that services postdating Presentation 3.0 (Search, AutoComplete, Discovery,
    ///     Content State, Auth 2.0) bind their constructor parameters against. A no-op when the token
    ///     already uses that shape.
    /// </summary>
    private static JToken WithUnprefixedIdType(JToken serviceToken)
    {
        if (serviceToken is not JObject { } obj || (obj["@id"] is null && obj["@type"] is null)) return serviceToken;

        var clone = (JObject)obj.DeepClone();
        RenameIfPresent(clone, "@id", "id");
        RenameIfPresent(clone, "@type", "type");
        return clone;
    }

    private static void RenameIfPresent(JObject obj, string oldName, string newName)
    {
        if (obj[oldName] is not { } value || obj[newName] is not null) return;

        obj.Remove(oldName);
        obj[newName] = value;
    }

    /// <summary>
    ///     Writes a service object to JSON.
    ///     Delegates to the standard serializer to use each service type's own converter.
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

    private sealed class LeafContractResolver : IIIFJsonContractResolver
    {
        protected override JsonConverter? ResolveContractConverter(Type objectType)
        {
            var converter = base.ResolveContractConverter(objectType);

            // Only suppress the converter for concrete leaf types (which inherit it via the
            // interface-attribute quirk noted above, and would otherwise recurse into
            // ServiceJsonConverter.ReadJson/WriteJson for themselves). Auth 2.0's services nest
            // *other* services polymorphically (e.g. AuthProbeService2.Service holding
            // AuthAccessService2), so when objectType is the interface itself, the converter must
            // stay active to dispatch to the right concrete type - otherwise Newtonsoft tries to
            // instantiate IBaseService directly and throws.
            return converter is ServiceJsonConverter && objectType != typeof(IBaseService) ? null : converter;
        }
    }
}