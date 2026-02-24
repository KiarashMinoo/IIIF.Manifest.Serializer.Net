using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
[method: JsonConstructor]
public class Related(string id) : FormattableItem<Related>(id);