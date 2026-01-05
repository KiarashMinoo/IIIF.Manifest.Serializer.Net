using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Content.Segment.Selector
{
    public class Selector : BaseItem<Selector>
    {
        public const string RegionJName = "region";

        [JsonProperty(RegionJName)]
        public List<int> Region { get; private set; }

        public Selector(string id, string type) : base(id, type)
        {
        }

        public Selector SetRegion(List<int> region) => SetPropertyValue(a => a.Region, region);
        public Selector SetRegion(int left, int top, int width, int height) => SetRegion(new List<int>(new[] { left, top, width, height }));
    }
}