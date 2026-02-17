using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Content.Audio;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.OtherContent;
using IIIF.Manifests.Serializer.Nodes.Content.Video;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Canvas
{
    /// <summary>
    /// IIIF Canvas resource - a virtual container representing a page or view.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Core resource. In 3.0, images property replaced by items (AnnotationPage).")]
    [JsonConverter(typeof(CanvasJsonConverter))]
    public class Canvas : BaseNode<Canvas>, IDimenssionSupport<Canvas>
    {
        public const string ImagesJName = "images";
        public const string OtherContentsJName = "otherContent";
        public const string DurationJName = "duration";

        private readonly List<Image> images = new List<Image>();
        private readonly List<Audio> audios = new List<Audio>();
        private readonly List<Video> videos = new List<Video>();
        private readonly List<OtherContent> otherContents = new List<OtherContent>();


        [PresentationAPI("2.0")]
        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; }

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; }

        /// <summary>
        /// Duration in seconds for time-based media (A/V content).
        /// </summary>
        [PresentationAPI("2.1", Notes = "Added in 2.1 for A/V support. Also in 3.0.")]
        [JsonProperty(DurationJName)]
        public double? Duration { get; private set; }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ImagesJName)]
        public IReadOnlyCollection<Image> Images => images.AsReadOnly();

        public IReadOnlyCollection<Audio> Audios => audios.AsReadOnly();
        public IReadOnlyCollection<Video> Videos => videos.AsReadOnly();

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "annotations")]
        [JsonProperty(OtherContentsJName)]
        public IReadOnlyCollection<OtherContent> OtherContents => otherContents.AsReadOnly();

        public Canvas(string id, Label label, int height, int width) : base(id, "sc:Canvas")
        {
            SetLabel(new[] { label });
            Height = height;
            Width = width;
        }

        public Canvas AddImage(Image image) => SetPropertyValue(a => a.images, a => a.Images, images.Attach(image));
        public Canvas AddAudio(Audio audio) => SetPropertyValue(a => a.audios, a => a.Audios, audios.Attach(audio));
        public Canvas AddVideo(Video video) => SetPropertyValue(a => a.videos, a => a.Videos, videos.Attach(video));
        public Canvas AddOtherContent(OtherContent otherContent) => SetPropertyValue(a => a.otherContents, a => a.OtherContents, otherContents.Attach(otherContent));

        public Canvas SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public Canvas SetWidth(int width) => SetPropertyValue(a => a.Width, width);
        public Canvas SetDuration(double duration) => SetPropertyValue(a => a.Duration, duration);
    }
}