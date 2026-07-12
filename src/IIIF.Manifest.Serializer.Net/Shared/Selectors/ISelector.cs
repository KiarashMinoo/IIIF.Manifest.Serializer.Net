using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Selectors;

/// <summary>
///     A W3C Web Annotation Selector - identifies a specific part of a resource (a region, a point in
///     time, an Image API transform, or an arbitrary polygon) referenced from a
///     <see cref="Content.Resources.SpecificResource" />'s <c>selector</c> property, wherever it
///     appears (an <c>Annotation</c>'s <c>target</c> or <c>body</c>).
/// </summary>
[JsonConverter(typeof(SelectorJsonConverter))]
public interface ISelector
{
    string Type { get; }
}