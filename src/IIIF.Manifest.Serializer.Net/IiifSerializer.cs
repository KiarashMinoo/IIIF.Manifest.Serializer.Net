using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
///     Version-aware SDK entry point for IIIF Presentation manifests. Acts as a thin Facade: this file
///     holds only the public <c>Serialize</c>/<c>Deserialize</c> overloads and their 2.x/3.0 version
///     dispatch; the hand-rolled V3 read/write logic for each resource type lives in its own
///     <c>partial class</c> file (<c>IiifSerializer.Manifest.cs</c>, <c>IiifSerializer.Canvas.cs</c>,
///     etc.) so no single file has to hold the whole serializer.
/// </summary>
public static partial class IiifSerializer
{
    public static string Serialize(Manifest manifest)
    {
        return Serialize(manifest, IiifSerializerOptions.Default);
    }

    public static string Serialize(Manifest manifest, IiifSerializerOptions? options)
    {
        if (manifest is null) throw new ArgumentNullException(nameof(manifest));

        options ??= IiifSerializerOptions.Default;

        return options.Version switch
        {
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.SerializeObject(manifest, TrackableObject.JsonSerializerSettings),
            IiifPresentationVersion.V3_0 => WriteV3Manifest(manifest).ToString(Formatting.Indented),
            _ => throw new NotSupportedException($"Unsupported IIIF Presentation API version: {options.Version}.")
        };
    }

    /// <summary>
    ///     Serializes only <paramref name="manifest" />'s pending changes (issue #23) - equivalent
    ///     to <c>Serialize(manifest.GetChangedManifest(), options)</c>, provided as a named entry
    ///     point so callers reaching for a "changed-only" API don't need to know about
    ///     <c>Manifest.GetChangedManifest()</c> separately. Full <see cref="Serialize(Manifest)" />
    ///     is entirely unaffected by this method's existence - it always serializes the complete
    ///     object graph regardless of tracked changes.
    /// </summary>
    public static string SerializeChangedOnly(Manifest manifest, IiifSerializerOptions? options = null)
    {
        if (manifest is null) throw new ArgumentNullException(nameof(manifest));

        return Serialize(manifest.GetChangedManifest(), options);
    }

    public static Manifest DeserializeManifest(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));

        var version = IiifPresentationVersionDetector.Detect(json);
        var manifest = version switch
        {
            IiifPresentationVersion.V3_0 => ReadV3Manifest(JObject.Parse(json)),
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.DeserializeObject<Manifest>(json, TrackableObject.JsonSerializerSettings)
                                                                            ?? throw new JsonSerializationException("Could not deserialize IIIF manifest."),
            IiifPresentationVersion.Unknown => throw new JsonSerializationException("Could not detect IIIF Presentation API version."),
            _ => throw new NotSupportedException($"Detected IIIF version '{version}', but this SDK does not import it as a Manifest. Supported versions: 2.0, 2.1, 3.0.")
        };

        // Change tracking (issue #23): a document freshly loaded from storage has no "pending
        // edits" yet - establish this as the clean baseline before handing it back, per
        // docs/CHANGE_TRACKING.md's "deserialization starts clean by default" decision.
        manifest.ClearChanges();
        return manifest;
    }

    public static string Serialize(Collection collection)
    {
        return Serialize(collection, IiifSerializerOptions.Default);
    }

    public static string Serialize(Collection collection, IiifSerializerOptions? options)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        options ??= IiifSerializerOptions.Default;

        return options.Version switch
        {
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.SerializeObject(collection, TrackableObject.JsonSerializerSettings),
            IiifPresentationVersion.V3_0 => WriteV3Collection(collection).ToString(Formatting.Indented),
            _ => throw new NotSupportedException($"Unsupported IIIF Presentation API version: {options.Version}.")
        };
    }

    public static Collection DeserializeCollection(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));

        var version = IiifPresentationVersionDetector.Detect(json);
        var collection = version switch
        {
            IiifPresentationVersion.V3_0 => ReadV3Collection(JObject.Parse(json)),
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.DeserializeObject<Collection>(json, TrackableObject.JsonSerializerSettings)
                                                                            ?? throw new JsonSerializationException("Could not deserialize IIIF collection."),
            IiifPresentationVersion.Unknown => throw new JsonSerializationException("Could not detect IIIF Presentation API version."),
            _ => throw new NotSupportedException($"Detected IIIF version '{version}', but this SDK does not import it as a Collection. Supported versions: 2.0, 2.1, 3.0.")
        };

        collection.ClearChanges(); // see DeserializeManifest's comment - same "clean by default" decision.
        return collection;
    }

    public static string Serialize(AnnotationCollection annotationCollection)
    {
        return Serialize(annotationCollection, IiifSerializerOptions.Default);
    }

    public static string Serialize(AnnotationCollection annotationCollection, IiifSerializerOptions? options)
    {
        if (annotationCollection is null) throw new ArgumentNullException(nameof(annotationCollection));

        options ??= IiifSerializerOptions.Default;

        return options.Version switch
        {
            IiifPresentationVersion.V3_0 => WriteV3AnnotationCollection(annotationCollection).ToString(Formatting.Indented),
            _ => throw new NotSupportedException($"Unsupported IIIF Presentation API version: {options.Version}. AnnotationCollection is a 3.0-only concept.")
        };
    }

    public static AnnotationCollection DeserializeAnnotationCollection(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));

        var annotationCollection = ReadV3AnnotationCollection(JObject.Parse(json));
        annotationCollection.ClearChanges(); // see DeserializeManifest's comment - same "clean by default" decision.
        return annotationCollection;
    }
}