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

    /// <summary>
    ///     The primary, spec-real attach point: <c>textGranularity</c> is the real
    ///     <see cref="Nodes.Contents.Annotation.Annotation" />'s own top-level property (a sibling
    ///     of <c>motivation</c>/<c>body</c>/<c>target</c>), not its body's. The <see cref="IBaseResource" />-typed
    ///     overload above pre-dates this one and models the property on a standalone
    ///     <c>BaseResource</c> tagged <see cref="ResourceType.Annotation" />; that shape never
    ///     actually occurs on a real Annotation embedded in a Manifest/Canvas tree, since
    ///     <see cref="Nodes.Contents.Annotation.Annotation" /> does not implement
    ///     <see cref="IBaseResource" />. This overload is what a caller building/reading a real
    ///     annotation (e.g. the issue's own "word"-granularity example) actually needs.
    /// </summary>
    extension(Nodes.Contents.Annotation.Annotation annotation)
    {
        [TextGranularityExtension("3.0")]
        public TextGranularity? TextGranularity => annotation.GetAdditionalProperty<Nodes.Contents.Annotation.Annotation, TextGranularity>(TextGranularity.TextGranularityJName);

        [TextGranularityExtension("3.0")]
        public Nodes.Contents.Annotation.Annotation SetTextGranularity(TextGranularity textGranularity)
        {
            return annotation.SetAdditionalProperty(TextGranularity.TextGranularityJName, textGranularity);
        }
    }
}