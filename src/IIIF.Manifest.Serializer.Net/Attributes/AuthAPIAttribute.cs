using System;

namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
/// Indicates this feature is part of IIIF Auth API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class AuthAPIAttribute : IIIFVersionAttribute
{
    public AuthAPIAttribute(string minVersion, string maxVersion = null) 
        : base(minVersion, maxVersion)
    {
    }
}