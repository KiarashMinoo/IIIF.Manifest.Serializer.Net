using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
///     A <see cref="GeoreferenceAnnotation" />'s target - the Canvas or Image Service being
///     georeferenced, optionally narrowed to a specific region via a <see cref="Selector" />. Serializes
///     as one of the 3 shapes the spec allows (see <see cref="GeoreferenceTargetJsonConverter" />): a
///     bare URI string, a full resource object (id/type/height/width), or a SpecificResource wrapping
///     a <see cref="GeoreferenceSvgSelector" />.
/// </summary>
[JsonConverter(typeof(GeoreferenceTargetJsonConverter))]
public class GeoreferenceTarget : TrackableObject<GeoreferenceTarget>
{
    public GeoreferenceTarget(string sourceId, string? sourceType = null)
    {
        SourceId = sourceId;
        SourceType = sourceType;
    }

    public string SourceId
    {
        get => GetElementValue(x => x.SourceId)!;
        private set => SetElementValue(value);
    }

    public string? SourceType
    {
        get => GetElementValue(x => x.SourceType);
        private set => SetElementValue(value);
    }

    public int? SourceHeight
    {
        get => GetElementValue(x => x.SourceHeight);
        private set => SetElementValue(value);
    }

    public int? SourceWidth
    {
        get => GetElementValue(x => x.SourceWidth);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     The SpecificResource wrapper's own id - only meaningful when <see cref="Selector" /> is set.
    /// </summary>
    public string? SpecificResourceId
    {
        get => GetElementValue(x => x.SpecificResourceId);
        private set => SetElementValue(value);
    }

    [GeoreferenceExtension("3.0")]
    public GeoreferenceSvgSelector? Selector
    {
        get => GetElementValue(x => x.Selector);
        private set => SetElementValue(value);
    }

    public GeoreferenceTarget SetSourceDimensions(int height, int width)
    {
        SourceHeight = height;
        SourceWidth = width;
        return this;
    }

    public GeoreferenceTarget SetSelector(GeoreferenceSvgSelector selector)
    {
        Selector = selector;
        return this;
    }

    public GeoreferenceTarget SetSpecificResourceId(string specificResourceId)
    {
        SpecificResourceId = specificResourceId;
        return this;
    }
}