using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF AccompanyingCanvas property - references a canvas that accompanies the manifest.
    /// </summary>
    [JsonConverter(typeof(AccompanyingCanvasJsonConverter))]
    public class AccompanyingCanvas : BaseItem<AccompanyingCanvas>
    {
        public AccompanyingCanvas(string id) : base(id, "sc:Canvas")
        {
        }
    }
}