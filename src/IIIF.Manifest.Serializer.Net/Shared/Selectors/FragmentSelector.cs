using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Selectors;

/// <summary>
///     A W3C "FragmentSelector" conforming to the Media Fragments spec - selects a spatial region
///     (<c>xywh=x,y,w,h</c>) or temporal range (<c>t=start,end</c>) of the targeted resource.
/// </summary>
public class FragmentSelector : TrackableObject<FragmentSelector>, ISelector
{
    public const string TypeJName = "type";
    public const string ConformsToJName = "conformsTo";
    public const string ValueJName = "value";
    public const string MediaFragmentConformsTo = "http://www.w3.org/TR/media-frags/";

    [JsonConstructor]
    public FragmentSelector(string value)
    {
        Type = "FragmentSelector";
        ConformsTo = MediaFragmentConformsTo;
        Value = value;
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

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "FragmentSelector";
        private set => SetElementValue(value);
    }

    public static FragmentSelector ForRegion(int x, int y, int width, int height)
    {
        return new FragmentSelector($"xywh={x},{y},{width},{height}");
    }
}