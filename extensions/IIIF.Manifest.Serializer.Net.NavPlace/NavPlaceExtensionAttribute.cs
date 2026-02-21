using System;
using IIIF.Manifests.Serializer.Attributes;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Attribute for IIIF navPlace extension properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NavPlaceExtensionAttribute(string version) : IIIFVersionAttribute(version);
}