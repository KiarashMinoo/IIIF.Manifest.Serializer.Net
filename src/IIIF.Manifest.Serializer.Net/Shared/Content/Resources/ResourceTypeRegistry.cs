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
/// <c>IiifSerializer</c>'s hand-rolled V3 Canvas reader. An extension type registers itself from its
/// own static constructor, which runs the first time that type is touched - typically already
/// guaranteed by the time a document referencing it needs to be read, since calling code must
/// construct/reference the type to have produced that document in the first place.
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
