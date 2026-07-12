namespace IIIF.Manifests.Serializer;

/// <summary>
///     Options for SDK-level IIIF serialization.
/// </summary>
public sealed class IiifSerializerOptions
{
    public IiifSerializerOptions(
        IiifPresentationVersion version = IiifPresentationVersion.V3_0,
        bool preserveLegacyProperties = false)
    {
        Version = version;
        PreserveLegacyProperties = preserveLegacyProperties;
    }

    public static IiifSerializerOptions Default { get; } = new();

    public IiifPresentationVersion Version { get; }

    public bool PreserveLegacyProperties { get; }
}