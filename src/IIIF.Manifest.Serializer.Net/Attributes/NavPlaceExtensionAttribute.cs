namespace IIIF.Manifests.Serializer.Attributes;

/// <summary>
///     Attribute to mark properties that are part of the navPlace extension.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NavPlaceExtensionAttribute : IIIFVersionAttribute
{
    public NavPlaceExtensionAttribute(string version) : base(version)
    {
    }
}