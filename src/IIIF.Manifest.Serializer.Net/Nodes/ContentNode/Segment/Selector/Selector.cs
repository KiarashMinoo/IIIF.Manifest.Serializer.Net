using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Segment.Selector
{
    public class Selector(string id, string type) : BaseItem<Selector>(id, type)
    {
        public const string RegionJName = "region";

        [JsonProperty(RegionJName)] public List<int> Region => GetElementValue(x => x.Region) ?? [];

        public Selector SetRegion(List<int> region) => SetElementValue(a => a.Region, region);
        public Selector SetRegion(int left, int top, int width, int height) => SetRegion([left, top, width, height]);
    }
}