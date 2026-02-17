using System;
using IIIF.Manifests.Serializer.Attributes;

namespace IIIF.Manifests.Serializer.Attributes
{
    /// <summary>
    /// Attribute to mark properties that are part of the navPlace extension.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NavPlaceExtensionAttribute : IIIFVersionAttribute
    {
        public NavPlaceExtensionAttribute(string version) : base(version) { }
    }
}