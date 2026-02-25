using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions;

public static class TextGranularityExtensions
{
    extension<TResource>(TResource resource) where TResource : IBaseResource, IAdditionalPropertiesSupport<TResource>
    {
        public TResource SetTextGranularity(TextGranularity textGranularity)
        {
            return resource.Type == ResourceType.Annotation
                ? resource.SetAdditionalProperty(TextGranularity.TextGranularityJName, textGranularity)
                : throw new InvalidOperationException("The textGranularity property is only valid for resources of type Annotation.");
        }

        public TextGranularity? TextGranularity => resource.Type == ResourceType.Annotation
            ? resource.GetAdditionalProperty<TResource, TextGranularity>(TextGranularity.TextGranularityJName)
            : null;
    }
}