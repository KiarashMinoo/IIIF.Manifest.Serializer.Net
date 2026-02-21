using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Audio;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.OtherContent;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Video;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.CanvasNode
{
    public class CanvasJsonConverter : BaseNodeJsonConverter<Canvas>
    {
        private Canvas ConstructCanvas(JToken element)
        {
            var jId = element.TryGetToken(Canvas.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Canvas>(Canvas.IdJName);

            var jLabel = element.TryGetToken(Canvas.LabelJName);
            if (jLabel is null)
                throw new JsonNodeRequiredException<Canvas>(Canvas.LabelJName);

            var jHeight = element.TryGetToken(Constants.HeightJName);
            if (jHeight is null)
                throw new JsonNodeRequiredException<Canvas>(Constants.HeightJName);

            var jWidth = element.TryGetToken(Constants.WidthJName);
            if (jWidth is null)
                throw new JsonNodeRequiredException<Canvas>(Constants.WidthJName);

            if (jLabel is JArray)
            {
                var labels = jLabel.ToObject<Label[]>();
                var first = labels[0];
                var canvas = new Canvas(jId.ToString(), first, jHeight.Value<int>(), jWidth.Value<int>());
                foreach (var label in labels.Except([first]))
                    canvas.AddLabel(label);
                return canvas;
            }

            return new Canvas(jId.ToString(), jLabel.ToObject<Label>(), jHeight.Value<int>(), jWidth.Value<int>());
        }

        private Canvas SetImages(JToken element, Canvas canvas)
        {
            var jImages = element.TryGetToken(Canvas.ImagesJName);
            if (jImages is null)
                return canvas;

            if (!(jImages is JArray))
                throw new JsonObjectMustBeJArray<Canvas>(Canvas.ImagesJName);

            foreach (var jImage in jImages)
            {
                var jResource = jImage.TryGetToken("resource");
                if (jResource != null)
                {
                    var jType = jResource.TryGetToken("@type");
                    if (jType != null)
                    {
                        var type = jType.ToString();
                        switch (type)
                        {
                            case "dctypes:Image":
                            {
                                var image = jImage.ToObject<Image>();
                                if (image != null)
                                    canvas.AddImage(image);
                                break;
                            }
                            case "dctypes:Sound":
                            {
                                var audio = jImage.ToObject<Audio>();
                                if (audio != null)
                                    canvas.AddAudio(audio);
                                break;
                            }
                            case "dctypes:MovingImage":
                            {
                                var video = jImage.ToObject<Video>();
                                if (video != null)
                                    canvas.AddVideo(video);
                                break;
                            }
                            default:
                            {
                                // Default to Image or handle other types
                                var image = jImage.ToObject<Image>();
                                if (image != null)
                                    canvas.AddImage(image);
                                break;
                            }
                        }
                    }
                    else
                    {
                        var image = jImage.ToObject<Image>();
                        if (image != null)
                            canvas.AddImage(image);
                    }
                }
            }

            return canvas;
        }

        private Canvas SetOtherContents(JToken element, Canvas canvas)
        {
            var jOtherContents = element.TryGetToken(Canvas.OtherContentsJName);
            if (jOtherContents != null)
            {
                if (!(jOtherContents is JArray))
                    throw new JsonObjectMustBeJArray<Canvas>(Canvas.OtherContentsJName);

                var otherContents = jOtherContents.ToObject<OtherContent[]>();
                if (otherContents != null)
                {
                    foreach (var otherContent in otherContents)
                        canvas.AddOtherContent(otherContent);
                }
            }

            return canvas;
        }

        protected override Canvas CreateInstance(JToken element, Type objectType, Canvas? existingValue, bool hasExistingValue, JsonSerializer serializer) => ConstructCanvas(element);

        protected override Canvas EnrichReadJson(Canvas canvas, JToken element, Type objectType, Canvas? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Clear labels set by constructor to avoid duplication when base.EnrichReadJson re-reads them
            canvas.SetLabel([]);
            canvas = base.EnrichReadJson(canvas, element, objectType, existingValue, hasExistingValue, serializer);

            canvas = SetImages(element, canvas);
            canvas = SetOtherContents(element, canvas);
            canvas = SetDuration(element, canvas);

            return canvas;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Canvas canvas, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, canvas, serializer);

            var allImages = canvas.Images.Cast<object>().Concat(canvas.Audios).Concat(canvas.Videos).ToList();
            if (allImages.Any())
            {
                writer.WritePropertyName(Canvas.ImagesJName);

                writer.WriteStartArray();

                foreach (var item in allImages)
                    serializer.Serialize(writer, item);

                writer.WriteEndArray();
            }

            if (canvas.OtherContents.Any())
            {
                writer.WritePropertyName(Canvas.OtherContentsJName);

                writer.WriteStartArray();

                foreach (var otherContent in canvas.OtherContents)
                    serializer.Serialize(writer, otherContent);

                writer.WriteEndArray();
            }

            if (canvas.Width != null)
            {
                writer.WritePropertyName(Constants.WidthJName);
                writer.WriteValue(canvas.Width.Value);
            }

            if (canvas.Height != null)
            {
                writer.WritePropertyName(Constants.HeightJName);
                writer.WriteValue(canvas.Height.Value);
            }

            if (canvas.Duration != null)
            {
                writer.WritePropertyName(Canvas.DurationJName);
                writer.WriteValue(canvas.Duration.Value);
            }
        }

        private Canvas SetDuration(JToken element, Canvas canvas)
        {
            var jDuration = element.TryGetToken(Canvas.DurationJName);
            if (jDuration != null)
                canvas.SetDuration(jDuration.Value<double>());

            return canvas;
        }
    }
}