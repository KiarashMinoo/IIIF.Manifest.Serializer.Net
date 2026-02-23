using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;

namespace IIIF.Manifests.Serializer.Properties.AccompanyingCanvasProperty;

/// <summary>
/// IIIF AccompanyingCanvas property - references a canvas that accompanies the manifest.
/// </summary>
[PresentationAPI("2.0")]
public class AccompanyingCanvas(string id) : BaseItem<AccompanyingCanvas>(id, "sc:Canvas");