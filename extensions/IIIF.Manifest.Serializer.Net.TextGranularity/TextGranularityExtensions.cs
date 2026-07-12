using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions;

public static class TextGranularityExtensions
{
    private static int isRegistered;

    /// <summary>
    ///     Registers Text Granularity extension hooks.
    ///     Safe to call multiple times.
    /// </summary>
    public static void Register()
    {
        _ = Interlocked.Exchange(ref isRegistered, 1);
    }

    extension<TResource>(TResource resource) where TResource : IBaseResource, IAdditionalPropertiesSupport<TResource>
    {
        [TextGranularityExtension("3.0")]
        public TextGranularity? TextGranularity => resource.Type == ResourceType.Annotation
            ? resource.GetAdditionalProperty<TResource, TextGranularity>(TextGranularity.TextGranularityJName)
            : null;

        [TextGranularityExtension("3.0")]
        public TResource SetTextGranularity(TextGranularity textGranularity)
        {
            return resource.Type == ResourceType.Annotation
                ? resource.SetAdditionalProperty(TextGranularity.TextGranularityJName, textGranularity)
                : throw new InvalidOperationException("The textGranularity property is only valid for resources of type Annotation.");
        }
    }
}