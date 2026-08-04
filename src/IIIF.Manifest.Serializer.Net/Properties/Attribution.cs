using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
[JsonConverter(typeof(ValuableItemJsonConverter<Attribution>))]
public sealed class Attribution(string value) : ValuableItem<Attribution>(value)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private Attribution() : this(null!)
    {
    }
}