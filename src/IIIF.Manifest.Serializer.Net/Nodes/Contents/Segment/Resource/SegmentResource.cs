using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource;

public sealed class SegmentResource(string id, ResourceType type) : BaseResource<SegmentResource>(id, type)
{
    public const string FullJName = "full";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private SegmentResource() : this(null!, default!)
    {
    }

    [JsonProperty(FullJName)]
    public BaseResource? Full
    {
        get => GetElementValue(x => x.Full);
        private set => SetElementValue(value);
    }

    public SegmentResource SetFull(BaseResource resource)
    {
        Full = resource;
        return this;
    }
}