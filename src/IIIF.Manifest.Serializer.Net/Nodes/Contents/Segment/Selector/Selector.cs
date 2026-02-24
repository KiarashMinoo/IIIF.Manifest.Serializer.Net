using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Segment.Selector
{
    public class Selector(string id, string type) : BaseItem<Selector>(id, type)
    {
        public const string RegionJName = "region";

        [JsonProperty(RegionJName)]
        public IReadOnlyCollection<int> Region
        {
            get => GetElementValue(x => x.Region) ?? [];
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
}