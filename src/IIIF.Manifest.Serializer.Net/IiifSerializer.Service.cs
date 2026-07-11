using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
    private static JObject WriteV3Service(IBaseService service)
    {
        var token = JObject.FromObject(service, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));
        Rename(token, "@id", "id");
        Rename(token, "@type", "type");
        return token;
    }

    /// <summary>
    /// Same as <see cref="WriteV3Service"/> but additionally strips "@context" - unlike a
    /// top-level Manifest.Services entry (which may legitimately declare its own context per
    /// Milestone 9), a service embedded inline on a content resource (e.g. an Image API service on
    /// a painting body) never carries one in any real cookbook recipe.
    /// </summary>
    private static JObject WriteV3EmbeddedResourceService(IBaseService service)
    {
        var token = WriteV3Service(service);
        token.Remove("@context");
        return token;
    }

    private static IBaseService? ReadV3Service(JObject obj)
    {
        // A V3 manifest's "services" array always writes id/type unprefixed (see WriteV3Service);
        // ServiceJsonConverter normalizes to whichever shape the detected leaf class needs, so no
        // renaming is required here.
        return obj.ToObject<IBaseService>();
    }
}
