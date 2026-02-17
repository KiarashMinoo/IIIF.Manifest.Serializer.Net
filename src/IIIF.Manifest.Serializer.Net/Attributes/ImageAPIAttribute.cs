using System;

namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
/// Indicates this feature is part of IIIF Image API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class ImageAPIAttribute : IIIFVersionAttribute
{
    public ImageAPIAttribute(string minVersion, string maxVersion = null) 
        : base(minVersion, maxVersion)
    {
    }
}