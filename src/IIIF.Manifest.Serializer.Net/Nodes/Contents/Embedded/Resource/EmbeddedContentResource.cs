using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource
{
    public class EmbeddedContentResource : BaseResource<EmbeddedContentResource>
    {
        public const string CharsJName = "chars";
        public const string LanguageJname = "language";

        [JsonProperty(CharsJName)]
        public string Chars
        {
            get => GetElementValue(x => x.Chars)!;
            private set => SetElementValue(value);
        }

        [JsonProperty(LanguageJname)]
        public string Language
        {
            get => GetElementValue(x => x.Language)!;
            private set => SetElementValue(value);
        }

        public EmbeddedContentResource(string chars, string language) : base("cnt:ContextAsText")
        {
            Chars = chars;
            Language = language;
        }
    }
}