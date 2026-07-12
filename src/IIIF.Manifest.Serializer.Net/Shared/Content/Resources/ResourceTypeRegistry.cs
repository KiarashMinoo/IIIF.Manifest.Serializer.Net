using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Content.Resources;

/// <summary>
/// Lets an extension assembly register its own <see cref="IBaseResource"/> implementer as a
/// recognizable <see cref="Nodes.Contents.Annotation.Annotation"/> body type, without core taking a
/// dependency on that extension. Used by navPlace's <c>Feature</c> (cookbook recipe
/// 0139-geolocate-canvas-fragment embeds a bare GeoJSON Feature directly as an Annotation body,
/// distinct from navPlace's usual Manifest/Canvas-level <c>navPlace</c> property). Consulted by both
/// <see cref="BaseResourceJsonConverter"/> (the generic JsonConvert/TrackableObject.Parse path) and
/// <c>IiifSerializer</c>'s hand-rolled V3 Canvas reader. Extension packages should expose explicit,
/// idempotent registration methods (for example <c>NavPlaceExtensions.Register()</c>) and call those
/// before deserializing documents that may contain extension body types.
/// </summary>
public static class ResourceTypeRegistry
{
    private static readonly ConcurrentDictionary<string, Func<JObject, IBaseResource>> Factories = new();

    public static void Register(string typeName, Func<JObject, IBaseResource> factory)
    {
        Factories[typeName] = factory;
    }

    public static IBaseResource? TryCreate(string typeName, JObject obj)
    {
        return Factories.TryGetValue(typeName, out var factory) ? factory(obj) : null;
    }
}
