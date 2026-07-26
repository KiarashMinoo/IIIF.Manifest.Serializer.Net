namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
///     Attribute for IIIF Georeference extension properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GeoreferenceExtensionAttribute : IIIFVersionAttribute
{
    public GeoreferenceExtensionAttribute(string version) : base(version)
    {
    }
}