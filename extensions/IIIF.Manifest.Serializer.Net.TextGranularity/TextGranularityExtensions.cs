using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;

namespace IIIF.Manifests.Serializer.Extensions;

public static class TextGranularityExtensions
{
    extension(Image image)
    {
        public Image SetTextGranularity(TextGranularity textGranularity)
        {
            return image;
        }
    }
}