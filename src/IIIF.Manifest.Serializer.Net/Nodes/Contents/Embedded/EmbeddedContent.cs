using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Embedded;

public sealed class EmbeddedContent : BaseContent<EmbeddedContent, EmbeddedContentResource>
{
    public const string OnJName = "on";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private EmbeddedContent()
    {
    }

    public EmbeddedContent(string id, EmbeddedContentResource resource, string on) : base(id, "oa:Annotation", resource)
    {
        On = on;
    }

    [JsonProperty(OnJName)]
    public string On
    {
        get => GetElementValue(x => x.On)!;
        private set => SetElementValue(value);
    }
}