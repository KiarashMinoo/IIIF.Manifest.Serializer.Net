using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [method: JsonConstructor]
    public class StartCanvas(string id) : BaseItem<StartCanvas>(id);
}