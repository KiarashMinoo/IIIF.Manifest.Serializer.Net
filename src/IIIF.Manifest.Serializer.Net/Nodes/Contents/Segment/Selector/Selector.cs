using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Segment.Selector;

public sealed class Selector(string id, string type) : BaseItem<Selector>(id, type)
{
    public const string RegionJName = "region";

    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Selector() : this(null!, null!)
    {
    }

    [JsonProperty(RegionJName)]
    public IReadOnlyCollection<int> Region
    {
        get => GetElementValue(x => x.Region);
        private set => SetElementValue(value);
    }

    public Selector SetRegion(IReadOnlyCollection<int> region)
    {
        Region = region;
        return this;
    }

    public Selector SetRegion(int left, int top, int width, int height)
    {
        return SetRegion([left, top, width, height]);
    }
}