using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
public class Logo : FormattableItem<Logo>, IDimensionSupport<Logo>
{
    [JsonConstructor]
    public Logo(string id) : base(id, "dctypes:Image")
    {
    }

    [JsonProperty(Constants.HeightJName)]
    public int? Height
    {
        get => GetElementValue(x => x.Height);
        private set => SetElementValue(value);
    }

    [JsonProperty(Constants.WidthJName)]
    public int? Width
    {
        get => GetElementValue(x => x.Width);
        private set => SetElementValue(value);
    }

    public Logo SetHeight(int height)
    {
        Height = height;
        return this;
    }

    public Logo SetWidth(int width)
    {
        Width = width;
        return this;
    }
}