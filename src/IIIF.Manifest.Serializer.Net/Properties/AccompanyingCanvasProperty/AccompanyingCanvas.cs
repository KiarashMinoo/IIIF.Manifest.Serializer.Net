using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.AccompanyingCanvasProperty
{
    /// <summary>
    /// IIIF AccompanyingCanvas property - references a canvas that accompanies the manifest.
    /// </summary>
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(AccompanyingCanvasJsonConverter))]
    public class AccompanyingCanvas(string id) : BaseItem<AccompanyingCanvas>(id, "sc:Canvas");
}