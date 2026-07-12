using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;

public class EmbeddedContentResource : BaseResource<EmbeddedContentResource>
{
    public const string CharsJName = "chars";
    public const string LanguageJname = "language";

    // No @id at all is the norm for an embedded "cnt:ContentAsText" body (it's a literal
    // inline value, not a dereferenceable resource) - previously this constructor
    // accidentally passed the type string itself as @id (via the single-arg base(id) overload)
    // and misspelled it ("ContextAsText"), so @type was never actually set at all.
    public EmbeddedContentResource(string chars, string language) : base(string.Empty, "cnt:ContentAsText")
    {
        Chars = chars;
        Language = language;
    }

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
}