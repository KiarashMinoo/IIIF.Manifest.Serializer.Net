using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Annotation
{
    /// <summary>
    /// IIIF Presentation API 3.0 Web Annotation. Represents a single annotation on a Canvas
    /// (typically painting an Image/Audio/Video resource). This is the 3.0-native replacement for
    /// the 2.x Image/Audio/Video/OtherContent annotation wrapper classes.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Web Annotation model. In 2.x, this concept was split across Canvas.images/audio/video wrappers.")]
    public class Annotation : BaseItem<Annotation>
    {
        public const string MotivationJName = "motivation";
        public const string BodyJName = "body";
        public const string TargetJName = "target";

        [JsonProperty(MotivationJName)]
        public string Motivation
        {
            get => GetElementValue(x => x.Motivation) ?? "painting";
            private set => SetElementValue(value);
        }

        [JsonProperty(BodyJName)]
        public IBaseResource Body
        {
            get => GetElementValue(x => x.Body)!;
            private set => SetElementValue(value);
        }

        [JsonProperty(TargetJName)]
        public string Target
        {
            get => GetElementValue(x => x.Target)!;
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public Annotation(string id, IBaseResource body, string target) : base(id, "Annotation")
        {
            Body = body;
            Target = target;
            Motivation = "painting";
        }

        public Annotation SetMotivation(string motivation)
        {
            Motivation = motivation;
            return this;
        }
    }
}
