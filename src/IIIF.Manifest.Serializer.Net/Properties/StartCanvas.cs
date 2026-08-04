using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[method: JsonConstructor]
public sealed class StartCanvas(string id) : BaseItem<StartCanvas>(id)
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private StartCanvas() : this(null!)
    {
    }
}