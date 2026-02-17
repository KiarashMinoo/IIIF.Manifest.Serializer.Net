using System;

namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
/// Indicates this feature is part of IIIF Presentation API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class PresentationAPIAttribute : IIIFVersionAttribute
{
    public PresentationAPIAttribute(string minVersion, string maxVersion = null) 
        : base(minVersion, maxVersion)
    {
    }
}