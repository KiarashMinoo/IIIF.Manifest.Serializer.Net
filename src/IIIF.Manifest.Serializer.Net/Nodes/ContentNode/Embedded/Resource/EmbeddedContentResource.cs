using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Embedded.Resource
{
    public class EmbeddedContentResource : BaseResource<EmbeddedContentResource>
    {
        public const string CharsJName = "chars";
        public const string LanguageJname = "language";

        [JsonProperty(CharsJName)]
        public string Chars { get; }

        [JsonProperty(LanguageJname)]
        public string Language { get; }

        public EmbeddedContentResource(string chars, string language) : base("cnt:ContextAsText")
        {
            Chars = chars;
            Language = language;
        }
    }
}