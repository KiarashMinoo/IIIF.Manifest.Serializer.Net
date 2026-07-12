using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
///     A Content State 1.0 "PointSelector" (§5.2) - selects a single point in time within an AV
///     recording, e.g. to deep-link a viewer to start playback partway through a Canvas.
/// </summary>
[ContentStateAPI("1.0")]
public class ContentStatePointSelector : TrackableObject<ContentStatePointSelector>
{
    public const string TypeJName = "type";
    public const string TJName = "t";

    [JsonConstructor]
    public ContentStatePointSelector(double t)
    {
        Type = "PointSelector";
        T = t;
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "PointSelector";
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     The time offset, in seconds, into the targeted recording.
    /// </summary>
    [JsonProperty(TJName)]
    public double T
    {
        get => GetElementValue(x => x.T);
        private set => SetElementValue(value);
    }
}