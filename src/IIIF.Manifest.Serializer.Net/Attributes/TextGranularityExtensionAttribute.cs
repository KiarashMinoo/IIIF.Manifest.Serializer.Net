using System;

namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
/// Indicates this feature is part of IIIF Text Granularity Extension.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class TextGranularityExtensionAttribute : IIIFVersionAttribute
{
    public TextGranularityExtensionAttribute(string minVersion, string maxVersion = null)
        : base(minVersion, maxVersion)
    {
    }
}