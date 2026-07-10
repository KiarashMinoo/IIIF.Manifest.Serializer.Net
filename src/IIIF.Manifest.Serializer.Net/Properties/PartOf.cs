using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Presentation API 3.0 partOf reference - an object array with id/type. Replaces the
    /// 2.x "within" property (a bare id + optional label, restructured into id + type).
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces within from API 2.x. Restructured to an object array with id/type.")]
    public class PartOf : BaseItem<PartOf>
    {
        [JsonConstructor]
        public PartOf(string id, string type) : base(id, type)
        {
        }
    }
}
