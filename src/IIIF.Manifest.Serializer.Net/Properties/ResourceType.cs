using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF resource type values.
    /// Can be used directly with static properties or by creating custom values.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Type values vary between 2.x (sc: prefix) and 3.0 (no prefix).")]
    [JsonConverter(typeof(ValuableItemJsonConverter<ResourceType>))]
    public class ResourceType : ValuableItem<ResourceType>
    {
        public ResourceType(string value) : base(value)
        {
        }

        // IIIF Presentation 3.0 types
        public static ResourceType Collection => new ResourceType("Collection");
        public static ResourceType Manifest => new ResourceType("Manifest");
        public static ResourceType Canvas => new ResourceType("Canvas");
        public static ResourceType Range => new ResourceType("Range");
        public static ResourceType AnnotationPage => new ResourceType("AnnotationPage");
        public static ResourceType Annotation => new ResourceType("Annotation");
        public static ResourceType AnnotationCollection => new ResourceType("AnnotationCollection");

        // Content types
        public static ResourceType Image => new ResourceType("Image");
        public static ResourceType Video => new ResourceType("Video");
        public static ResourceType Sound => new ResourceType("Sound");
        public static ResourceType Text => new ResourceType("Text");
        public static ResourceType Dataset => new ResourceType("Dataset");
        public static ResourceType Model => new ResourceType("Model");

        // Service types
        public static ResourceType ImageService2 => new ResourceType("ImageService2");
        public static ResourceType ImageService3 => new ResourceType("ImageService3");
        public static ResourceType SearchService2 => new ResourceType("SearchService2");
        public static ResourceType AutoCompleteService2 => new ResourceType("AutoCompleteService2");
        public static ResourceType AuthCookieService1 => new ResourceType("AuthCookieService1");
        public static ResourceType AuthTokenService1 => new ResourceType("AuthTokenService1");
        public static ResourceType AuthLogoutService1 => new ResourceType("AuthLogoutService1");
        public static ResourceType AuthProbeService2 => new ResourceType("AuthProbeService2");
        public static ResourceType AuthAccessService2 => new ResourceType("AuthAccessService2");
        public static ResourceType AuthAccessTokenService2 => new ResourceType("AuthAccessTokenService2");
        public static ResourceType AuthLogoutService2 => new ResourceType("AuthLogoutService2");

        // IIIF 2.0 types (with prefix)
        public static ResourceType ScCollection => new ResourceType("sc:Collection");
        public static ResourceType ScManifest => new ResourceType("sc:Manifest");
        public static ResourceType ScSequence => new ResourceType("sc:Sequence");
        public static ResourceType ScCanvas => new ResourceType("sc:Canvas");
        public static ResourceType ScRange => new ResourceType("sc:Range");
        public static ResourceType OaAnnotation => new ResourceType("oa:Annotation");
        public static ResourceType DctypesImage => new ResourceType("dctypes:Image");
    }
}
