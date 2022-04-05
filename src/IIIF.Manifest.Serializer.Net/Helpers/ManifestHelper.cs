using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties;
using System.Collections.Generic;
using System.Linq;

namespace IIIF.Manifests.Serializer.Helpers
{
    public static class ManifestHelper
    {
        public static Manifest SetMetadata(this Manifest manifest, string label, string value, string language = null)
        {
            var metadata = manifest.Metadata.FirstOrDefault(a => a.Label == label);
            if (metadata != null)
            {
                if (language != null)
                {
                    var metadataValue = metadata.Value.FirstOrDefault(a => a.Language == language);
                    if (metadataValue != null)
                        metadataValue.SetValue(value);
                    else
                        metadata.AddValue(value, language);
                }
                else
                    metadata.ResetValue(value);
            }
            else
            {
                if (language != null)
                    manifest.AddMetadata(new Metadata(label, value, language));
                else
                    manifest.AddMetadata(new Metadata(label, value));
            }

            return manifest;
        }

        public static IReadOnlyCollection<MetadataValue> GetMetadata(this Manifest manifest, string label)
            => manifest.Metadata.FirstOrDefault(metadata => metadata.Label == label)?.Value;
    }
}