using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
/// IIIF Content State API 1.0 - a compact, encodable description of a "state" (which resource(s),
/// and optionally which region of them, a user was looking at) for deep linking. Modeled as a W3C
/// Annotation with motivation "contentState", per spec. Use <see cref="ContentStateCodec"/> to
/// encode/decode the base64url "iiif-content" string this object represents.
/// </summary>
[ContentStateAPI("1.0")]
[System.Text.Json.Serialization.JsonConverter(typeof(SystemTextJson.ContentStateSystemTextJsonConverter))]
public class ContentState : TrackableObject<ContentState>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string MotivationJName = "motivation";
    public const string TargetJName = "target";

    [ContentStateAPI("1.0")]
    [JsonProperty(IdJName)]
    public string? Id
    {
        get => GetElementValue(x => x.Id);
        private set => SetElementValue(value);
    }

    [ContentStateAPI("1.0")]
    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "Annotation";
        private set => SetElementValue(value);
    }

    [ContentStateAPI("1.0")]
    [JsonProperty(MotivationJName)]
    public string Motivation
    {
        get => GetElementValue(x => x.Motivation) ?? "contentState";
        private set => SetElementValue(value);
    }

    [ContentStateAPI("1.0")]
    [JsonProperty(TargetJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<ContentStateTarget> Target
    {
        get => GetElementValue(x => x.Target) ?? [];
        private set => SetElementValue(value);
    }

    public ContentState(params ContentStateTarget[] targets)
    {
        Type = "Annotation";
        Motivation = "contentState";
        Target = targets;
    }

    public ContentState SetId(string id)
    {
        Id = id;
        return this;
    }

    public ContentState AddTarget(ContentStateTarget target)
    {
        Target = Target.With(target);
        return this;
    }

    public ContentState RemoveTarget(ContentStateTarget target)
    {
        Target = Target.Without(target);
        return this;
    }
}
