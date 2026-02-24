using IIIF.Manifests.Serializer.Nodes.Contents.Image;

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