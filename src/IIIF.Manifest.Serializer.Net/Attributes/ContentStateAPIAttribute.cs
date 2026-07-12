namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
///     Indicates this feature is part of IIIF Content State API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
public class ContentStateAPIAttribute(string minVersion, string? maxVersion = null) : IIIFVersionAttribute(minVersion, maxVersion);