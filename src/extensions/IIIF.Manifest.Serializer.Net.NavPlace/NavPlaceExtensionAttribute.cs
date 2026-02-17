using System;
using IIIF.Manifests.Serializer.Attributes;

namespace IIIF.Manifests.Serializer.NavPlace
{
    /// <summary>
    /// Attribute for IIIF navPlace extension properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NavPlaceExtensionAttribute : IIIFVersionAttribute
    {
        public NavPlaceExtensionAttribute(string version) : base(version)
        {
        }
    }
}