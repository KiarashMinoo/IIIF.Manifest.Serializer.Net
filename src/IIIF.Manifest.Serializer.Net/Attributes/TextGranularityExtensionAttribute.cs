namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
///     Indicates this feature is part of IIIF Text Granularity Extension.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
public class TextGranularityExtensionAttribute(string minVersion, string? maxVersion = null) : IIIFVersionAttribute(minVersion, maxVersion);