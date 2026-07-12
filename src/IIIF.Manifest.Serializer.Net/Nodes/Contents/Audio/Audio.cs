using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Audio;

public class Audio : BaseContent<Audio, AudioResource>
{
    public const string MotivationJName = "motivation";
    public const string OnJName = "on";

    [JsonConstructor]
    public Audio(string id, AudioResource resource, string on) : base(id, "oa:Annotation", resource)
    {
        Motivation = "sc:painting";
        On = on;
    }

    [JsonProperty(MotivationJName)]
    public string Motivation
    {
        get => GetElementValue(x => x.Motivation)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(OnJName)]
    public string On
    {
        get => GetElementValue(x => x.On)!;
        private set => SetElementValue(value);
    }
}