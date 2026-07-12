using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
///     Extension methods for adding navPlace geographic location support to IIIF resources.
///     Per the navPlace spec, navPlace can be used with: Collection, Manifest, Range, Canvas.
///     Other types of resource must not have the navPlace property.
/// </summary>
public static class NavPlaceExtensions
{
    private static int isRegistered;

    /// <summary>
    ///     Registers navPlace extension resource/body types with core serializers.
    ///     Safe to call multiple times.
    /// </summary>
    public static void Register()
    {
        if (Interlocked.Exchange(ref isRegistered, 1) == 1) return;

        Feature.RegisterResourceType();
    }

    extension<TNode>(TNode node) where TNode : BaseNode<TNode>, IAdditionalPropertiesSupport<TNode>
    {
        /// <summary>
        ///     Get the navPlace property from this resource.
        /// </summary>
        [NavPlaceExtension("2.0")]
        public NavPlace? NavPlace => node.GetAdditionalProperty<TNode, NavPlace>(NavPlace.NavPlaceJName);

        /// <summary>
        ///     Set the navPlace property on this resource.
        /// </summary>
        public TNode SetNavPlace(NavPlace navPlace)
        {
            return node.SetAdditionalProperty(NavPlace.NavPlaceJName, navPlace);
        }
    }
}