using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Embedded.Resource
{
    public class EmbeddedContentResource : BaseResource<EmbeddedContentResource>
    {
        public const string CharsJName = "chars";
        public const string LanguageJname = "language";

        [JsonProperty(CharsJName)] public string Chars => GetElementValue(x => x.Chars)!;

        [JsonProperty(LanguageJname)] public string Language => GetElementValue(x => x.Language)!;

        public EmbeddedContentResource(string chars, string language) : base("cnt:ContextAsText")
        {
            SetElementValue(x => x.Chars, chars);
            SetElementValue(x => x.Language, language);
        }
    }
}