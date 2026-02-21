using System;
using IIIF.Manifests.Serializer.Attributes;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Attribute for IIIF Text Granularity extension properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TextGranularityExtensionAttribute : IIIFVersionAttribute
    {
        public TextGranularityExtensionAttribute(string version) : base(version)
        {
        }
    }
}