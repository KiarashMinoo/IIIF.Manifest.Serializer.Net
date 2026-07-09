using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.OtherContent;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    /// <summary>
    /// IIIF Canvas resource - a virtual container representing a page or view.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Core resource. In 3.0, images property replaced by items (AnnotationPage).")]
    public class Canvas : BaseNode<Canvas>, IDimensionSupport<Canvas>
    {
        public const string ImagesJName = "images";
        public const string OtherContentsJName = "otherContent";
        public const string DurationJName = "duration";

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.HeightJName)]
        public int? Height
        {
            get => GetElementValue<int>();
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.WidthJName)]
        public int? Width
        {
            get => GetElementValue<int>();
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Duration in seconds for time-based media (A/V content).
        /// </summary>
        [PresentationAPI("2.1", Notes = "Added in 2.1 for A/V support. Also in 3.0.")]
        [JsonProperty(DurationJName)]
        public double? Duration
        {
            get => GetElementValue<double>();
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(ImagesJName)]
        public IReadOnlyCollection<Image> Images
        {
            get => GetElementValue(x => x.Images) ?? [];
            private set => SetElementValue(value);
        }

        [JsonProperty(nameof(Audios))]
        public IReadOnlyCollection<Audio> Audios
        {
            get => GetElementValue(x => x.Audios) ?? [];
            private set => SetElementValue(value);
        }

        [JsonProperty(nameof(Videos))]
        public IReadOnlyCollection<Video> Videos
        {
            get => GetElementValue(x => x.Videos) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "annotations")]
        [JsonProperty(OtherContentsJName)]
        public IReadOnlyCollection<OtherContent> OtherContents
        {
            get => GetElementValue(x => x.OtherContents) ?? [];
            private set => SetElementValue(value);
        }

        public Canvas(string id, Label label, int height, int width) : base(id, "sc:Canvas")
        {
            AddLabel(label);
            Height = height;
            Width = width;
        }

        public Canvas AddImage(Image image)
        {
            Images = Images.With(image);
            return this;
        }

        public Canvas AddAudio(Audio audio)
        {
            Audios = Audios.With(audio);
            return this;
        }

        public Canvas AddVideo(Video video)
        {
            Videos = Videos.With(video);
            return this;
        }

        public Canvas AddOtherContent(OtherContent otherContent)
        {
            OtherContents = OtherContents.With(otherContent);
            return this;
        }

        public Canvas SetDuration(double duration)
        {
            Duration = duration;
            return this;
        }
    }
}


