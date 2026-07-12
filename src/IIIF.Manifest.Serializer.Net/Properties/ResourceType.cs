using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

/// <summary>
///     IIIF resource type values.
///     Can be used directly with static properties or by creating custom values.
/// </summary>
[PresentationAPI("2.0", Notes = "Type values vary between 2.x (sc: prefix) and 3.0 (no prefix).")]
[JsonConverter(typeof(ValuableItemJsonConverter<ResourceType>))]
public class ResourceType(string value) : ValuableItem<ResourceType>(value)
{
    // IIIF Presentation 3.0 types
    public static ResourceType Collection => new("Collection");
    public static ResourceType Manifest => new("Manifest");
    public static ResourceType Canvas => new("Canvas");
    public static ResourceType Range => new("Range");
    public static ResourceType AnnotationPage => new("AnnotationPage");
    public static ResourceType Annotation => new("Annotation");
    public static ResourceType AnnotationCollection => new("AnnotationCollection");

    // Content types
    public static ResourceType Image => new("dctypes:Image");
    public static ResourceType Video => new("dctypes:MovingImage");
    public static ResourceType Sound => new("dctypes:Sound");
    public static ResourceType Text => new("Text");
    public static ResourceType Dataset => new("Dataset");
    public static ResourceType Model => new("Model");

    // Service types
    public static ResourceType ImageService2 => new("ImageService2");
    public static ResourceType ImageService3 => new("ImageService3");
    public static ResourceType SearchService2 => new("SearchService2");
    public static ResourceType AutoCompleteService2 => new("AutoCompleteService2");
    public static ResourceType AuthCookieService1 => new("AuthCookieService1");
    public static ResourceType AuthTokenService1 => new("AuthTokenService1");
    public static ResourceType AuthLogoutService1 => new("AuthLogoutService1");
    public static ResourceType AuthProbeService2 => new("AuthProbeService2");
    public static ResourceType AuthAccessService2 => new("AuthAccessService2");
    public static ResourceType AuthAccessTokenService2 => new("AuthAccessTokenService2");
    public static ResourceType AuthLogoutService2 => new("AuthLogoutService2");

    // IIIF 2.0 types (with prefix)
    public static ResourceType ScCollection => new("sc:Collection");
    public static ResourceType ScManifest => new("sc:Manifest");
    public static ResourceType ScSequence => new("sc:Sequence");
    public static ResourceType ScCanvas => new("sc:Canvas");
    public static ResourceType ScRange => new("sc:Range");
    public static ResourceType OaAnnotation => new("oa:Annotation");
    public static ResourceType DctypesImage => new("dctypes:Image");
}