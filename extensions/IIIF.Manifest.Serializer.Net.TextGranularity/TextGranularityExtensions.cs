using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions;

public static class TextGranularityExtensions
{
    extension<TResource>(TResource resource) where TResource : IBaseResource, IAdditionalPropertiesSupport<TResource>
    {
        public TResource SetTextGranularity(TextGranularity textGranularity)
        {
            return resource;
        }

        public TextGranularity? TextGranularity => resource.GetAdditionalProperty<TResource, TextGranularity>(TextGranularity.TextGranularityJName);
    }
}