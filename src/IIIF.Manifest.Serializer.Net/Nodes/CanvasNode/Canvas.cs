using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Audio;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.OtherContent;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Video;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.CanvasNode
{
    /// <summary>
    /// IIIF Canvas resource - a virtual container representing a page or view.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Core resource. In 3.0, images property replaced by items (AnnotationPage).")]
    [JsonConverter(typeof(CanvasJsonConverter))]
    public class Canvas : BaseNode<Canvas>, IDimensionSupport<Canvas>
    {
        public const string ImagesJName = "images";
        public const string OtherContentsJName = "otherContent";
        public const string DurationJName = "duration";

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.HeightJName)]
        public int? Height => GetElementValue(x => x.Height);

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.WidthJName)]
        public int? Width => GetElementValue(x => x.Width);

        /// <summary>
        /// Duration in seconds for time-based media (A/V content).
        /// </summary>
        [PresentationAPI("2.1", Notes = "Added in 2.1 for A/V support. Also in 3.0.")]
        [JsonProperty(DurationJName)]
        public double? Duration => GetElementValue(x => x.Duration);

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ImagesJName)]
        public IReadOnlyCollection<Image> Images => GetElementValue(x => x.Images) ?? [];

        public IReadOnlyCollection<Audio> Audios => GetElementValue(x => x.Audios) ?? [];
        public IReadOnlyCollection<Video> Videos => GetElementValue(x => x.Videos) ?? [];

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "annotations")]
        [JsonProperty(OtherContentsJName)]
        public IReadOnlyCollection<OtherContent> OtherContents => GetElementValue(x => x.OtherContents) ?? [];

        public Canvas(string id, Label label, int height, int width) : base(id, "sc:Canvas")
        {
            SetLabel([label]);
            SetElementValue(x => x.Height, height);
            SetElementValue(x => x.Width, width);
        }

        public Canvas AddImage(Image image) => SetElementValue(a => a.Images, (collection) => collection.With(image));
        public Canvas AddAudio(Audio audio) => SetElementValue(a => a.Audios, (collection) => collection.With(audio));
        public Canvas AddVideo(Video video) => SetElementValue(a => a.Videos, (collection) => collection.With(video));
        public Canvas AddOtherContent(OtherContent otherContent) => SetElementValue(a => a.OtherContents, (collection) => collection.With(otherContent));

        public Canvas SetHeight(int height) => SetElementValue(a => a.Height, height);
        public Canvas SetWidth(int width) => SetElementValue(a => a.Width, width);
        public Canvas SetDuration(double duration) => SetElementValue(a => a.Duration, duration);
    }
}