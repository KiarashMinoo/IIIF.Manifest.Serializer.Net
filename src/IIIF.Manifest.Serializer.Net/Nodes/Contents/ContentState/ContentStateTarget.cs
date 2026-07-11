using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
/// A Content State 1.0 target - the resource (and optionally a region of it) a content state
/// points at. Serializes as one of the three shapes the spec allows, chosen automatically by
/// <see cref="ContentStateTargetJsonConverter"/> based on which fields are set: a bare URI string
/// (just <see cref="Id"/>), a typed resource reference (<see cref="Id"/> + <see cref="ResourceType"/>),
/// or a full SpecificResource wrapping a <see cref="Selector"/> and/or a <see cref="PartOfId"/> Manifest.
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

    [ContentStateAPI("1.0")]
    public ContentStateFragmentSelector? Selector
    {
        get => GetElementValue(x => x.Selector);
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

    public ContentStateTarget SetSelector(ContentStateFragmentSelector selector)
    {
        Selector = selector;
        return this;
    }

    public ContentStateTarget SetSelector(int x, int y, int width, int height) =>
        SetSelector(ContentStateFragmentSelector.ForRegion(x, y, width, height));

    public ContentStateTarget SetPartOf(string partOfId, string partOfType = "Manifest")
    {
        PartOfId = partOfId;
        PartOfType = partOfType;
        return this;
    }
}
