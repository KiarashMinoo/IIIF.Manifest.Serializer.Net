using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
[JsonConverter(typeof(ValuableItemJsonConverter<License>))]
public sealed class License(string value) : ValuableItem<License>(value);