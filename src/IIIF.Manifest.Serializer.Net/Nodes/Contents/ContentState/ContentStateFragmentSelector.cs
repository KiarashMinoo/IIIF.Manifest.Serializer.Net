using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
/// The Media Fragments region selector a Content State 1.0 target uses to point at a specific
/// region of a Canvas/image (the only selector shape this SDK models for Content State targets).
/// </summary>
[ContentStateAPI("1.0")]
public class ContentStateFragmentSelector : TrackableObject<ContentStateFragmentSelector>
{
    public const string TypeJName = "type";
    public const string ConformsToJName = "conformsTo";
    public const string ValueJName = "value";
    public const string MediaFragmentConformsTo = "http://www.w3.org/TR/media-frags/";

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "FragmentSelector";
        private set => SetElementValue(value);
    }

    [JsonProperty(ConformsToJName)]
    public string ConformsTo
    {
        get => GetElementValue(x => x.ConformsTo) ?? MediaFragmentConformsTo;
        private set => SetElementValue(value);
    }

    [JsonProperty(ValueJName)]
    public string Value
    {
        get => GetElementValue(x => x.Value)!;
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public ContentStateFragmentSelector(string value)
    {
        Type = "FragmentSelector";
        ConformsTo = MediaFragmentConformsTo;
        Value = value;
    }

    public static ContentStateFragmentSelector ForRegion(int x, int y, int width, int height) =>
        new($"xywh={x},{y},{width},{height}");
}
