using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
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