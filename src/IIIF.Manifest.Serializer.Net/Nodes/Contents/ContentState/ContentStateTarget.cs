using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
/// A Content State 1.0 target - the resource (and optionally a point in time within it) a content
/// state points at. Serializes as one of the three shapes the spec allows, chosen automatically by
/// <see cref="ContentStateTargetJsonConverter"/> based on which fields are set: a bare URI string
/// (just <see cref="Id"/>), a typed resource reference (<see cref="Id"/> + <see cref="ResourceType"/>),
/// or a full SpecificResource wrapping a <see cref="PointSelector"/> and/or a <see cref="PartOfId"/>
/// Manifest. Region-targeting (spec §5.1) has no SpecificResource/selector form at all - the spec's
/// own example expresses it as a Media Fragments suffix on the plain <see cref="Id"/> URI itself
/// (e.g. <c>"https://example.org/canvas7#xywh=1000,2000,1000,2000"</c>), which the bare-string
/// constructor already supports directly.
/// </summary>
[ContentStateAPI("1.0")]
[JsonConverter(typeof(ContentStateTargetJsonConverter))]
public class ContentStateTarget : TrackableObject<ContentStateTarget>
{
    [ContentStateAPI("1.0")]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    [ContentStateAPI("1.0")]
    public string? ResourceType
    {
        get => GetElementValue(x => x.ResourceType);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Selects a single point in time within the targeted AV recording (spec §5.2).
    /// </summary>
    [ContentStateAPI("1.0")]
    public ContentStatePointSelector? PointSelector
    {
        get => GetElementValue(x => x.PointSelector);
        private set => SetElementValue(value);
    }

    [ContentStateAPI("1.0")]
    public string? PartOfId
    {
        get => GetElementValue(x => x.PartOfId);
        private set => SetElementValue(value);
    }

    [ContentStateAPI("1.0")]
    public string? PartOfType
    {
        get => GetElementValue(x => x.PartOfType);
        private set => SetElementValue(value);
    }

    public ContentStateTarget(string id, string? resourceType = null)
    {
        Id = id;
        ResourceType = resourceType;
    }

    public ContentStateTarget SetPointSelector(ContentStatePointSelector pointSelector)
    {
        PointSelector = pointSelector;
        return this;
    }

    public ContentStateTarget SetPointSelector(double t) => SetPointSelector(new ContentStatePointSelector(t));

    public ContentStateTarget SetPartOf(string partOfId, string partOfType = "Manifest")
    {
        PartOfId = partOfId;
        PartOfType = partOfType;
        return this;
    }
}
