using System;

namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
/// Indicates this feature is part of IIIF Search API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class SearchAPIAttribute : IIIFVersionAttribute
{
    public SearchAPIAttribute(string minVersion, string maxVersion = null) 
        : base(minVersion, maxVersion)
    {
    }
}