using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Image;

public sealed class Image : BaseContent<Image, ImageResource>
{
    public const string MotivationJName = "motivation";
    public const string OnJName = "on";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Image()
    {
    }

    public Image(string id, ImageResource resource, string on) : base(id, "oa:Annotation", resource)
    {
        Motivation = "sc:painting";
        On = on;
    }

    [JsonProperty(MotivationJName)]
    public string Motivation
    {
        get => GetElementValue(x => x.Motivation)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(OnJName)]
    public string On
    {
        get => GetElementValue(x => x.On)!;
        private set => SetElementValue(value);
    }
}