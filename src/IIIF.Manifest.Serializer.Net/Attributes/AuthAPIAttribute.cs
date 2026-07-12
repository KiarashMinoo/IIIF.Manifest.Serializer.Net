namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
///     Indicates this feature is part of IIIF Auth API.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
public class AuthAPIAttribute(string minVersion, string? maxVersion = null) : IIIFVersionAttribute(minVersion, maxVersion);