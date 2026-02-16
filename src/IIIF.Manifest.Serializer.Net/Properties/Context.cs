using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF context URL values.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [IIIFVersion("1.0", Notes = "Context URLs identify the API version being used.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Context>))]
    public class Context : ValuableItem<Context>
    {
        public Context(string value) : base(value)
        {
        }

        // IIIF Presentation API contexts
        public static Context Presentation2 => new Context("http://iiif.io/api/presentation/2/context.json");
        public static Context Presentation3 => new Context("http://iiif.io/api/presentation/3/context.json");

        // IIIF Image API contexts
        public static Context Image2 => new Context("http://iiif.io/api/image/2/context.json");
        public static Context Image3 => new Context("http://iiif.io/api/image/3/context.json");

        // IIIF Auth API contexts
        public static Context Auth1 => new Context("http://iiif.io/api/auth/1/context.json");
        public static Context Auth2 => new Context("http://iiif.io/api/auth/2/context.json");

        // IIIF Search API contexts
        public static Context Search1 => new Context("http://iiif.io/api/search/1/context.json");
        public static Context Search2 => new Context("http://iiif.io/api/search/2/context.json");

        // IIIF Change Discovery API context
        public static Context Discovery1 => new Context("http://iiif.io/api/discovery/1/context.json");

        // IIIF Content State context
        public static Context ContentState1 => new Context("http://iiif.io/api/content-state/1/context.json");

        // W3C Annotation context
        public static Context WebAnnotation => new Context("http://www.w3.org/ns/anno.jsonld");
    }
}
