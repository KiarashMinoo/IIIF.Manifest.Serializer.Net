using System;
using IIIF.Manifests.Serializer.Attributes;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Attribute for IIIF Georeference extension properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class GeoreferenceExtensionAttribute : IIIFVersionAttribute
    {
        public GeoreferenceExtensionAttribute(string version) : base(version)
        {
        }
    }
}