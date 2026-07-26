using System.Diagnostics.CodeAnalysis;
using IIIF.Manifests.Serializer.ChangeTracking;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Core;

public partial class TrackableObject
{
    [JsonIgnore] internal readonly Dictionary<string, ElementDescriptor> ElementDescriptors = [];

    protected internal static JsonSerializerSettings JsonSerializerSettings { get; } = new()
    {
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ContractResolver = new IIIFJsonContractResolver()
    };

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this, JsonSerializerSettings);
    }

    public static TTrackableObject Parse<TTrackableObject>(string json)
        where TTrackableObject : TrackableObject
    {
        return !TryParse<TTrackableObject>(json, out var trackableObject)
            ? throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json))
            : trackableObject;
    }

    public static bool TryParse<TTrackableObject>(string json, [MaybeNullWhen(false)] out TTrackableObject trackableObject)
        where TTrackableObject : TrackableObject
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            trackableObject = null;
            return false;
        }

        trackableObject = JsonConvert.DeserializeObject<TTrackableObject>(json, JsonSerializerSettings);

        // Change tracking (issue #23): a document freshly loaded from storage has no "pending
        // edits" yet - establish this as the clean baseline, per docs/CHANGE_TRACKING.md's
        // "deserialization starts clean by default" decision. TTrackableObject is only constrained
        // to the non-generic TrackableObject here (Parse<T>/TryParse<T> serve every trackable type,
        // including this base class itself), so the change-tracking interface is checked at
        // runtime rather than via a compile-time generic constraint.
        (trackableObject as IIiifChangeTrackable)?.ClearChanges();

        return trackableObject is not null;
    }
}