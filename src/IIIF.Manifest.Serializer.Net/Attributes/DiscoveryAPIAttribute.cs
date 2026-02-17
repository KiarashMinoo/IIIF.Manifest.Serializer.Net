using System;

namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
/// Indicates this feature is part of IIIF Change Discovery API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class DiscoveryAPIAttribute : IIIFVersionAttribute
{
    public DiscoveryAPIAttribute(string minVersion, string maxVersion = null) 
        : base(minVersion, maxVersion)
    {
    }
}