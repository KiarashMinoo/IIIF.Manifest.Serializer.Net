using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace IIIF.Manifests.Serializer.Nodes
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
                foreach (var label in labels.Except(new[] { first }))
                    canvas.AddLabel(label);
                return canvas;
            }

            return new Canvas(jId.ToString(), jLabel.ToObject<Label>(), jHeight.Value<int>(), jWidth.Value<int>());
        }

        private Canvas SetImages(JToken element, Canvas canvas)
        {
            var jImages = element.TryGetToken(Canvas.ImagesJName);
            if (jImages is null)
                throw new JsonNodeRequiredException<Canvas>(Canvas.ImagesJName);

            if (!(jImages is JArray))
                throw new JsonObjectMustBeJArray<Canvas>(Canvas.ImagesJName);

            var images = jImages.ToObject<Image[]>();
            foreach (var image in images)
                canvas.AddImage(image);

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
                foreach (var otherContent in otherContents)
                    canvas.AddOtherContent(otherContent);
            }

            return canvas;
        }

        protected override Canvas CreateInstance(JToken element, Type objectType, Canvas existingValue, bool hasExistingValue, JsonSerializer serializer) => ConstructCanvas(element);

        protected override Canvas EnrichReadJson(Canvas canvas, JToken element, Type objectType, Canvas existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            canvas = base.EnrichReadJson(canvas, element, objectType, existingValue, hasExistingValue, serializer);

            canvas = SetImages(element, canvas);
            canvas = SetOtherContents(element, canvas);

            return canvas;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Canvas canvas, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, canvas, serializer);

            if (canvas != null)
            {
                if (canvas.Images.Any())
                {
                    writer.WritePropertyName(Canvas.ImagesJName);

                    writer.WriteStartArray();

                    foreach (var image in canvas.Images)
                        serializer.Serialize(writer, image);

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

                if (canvas.Label.Any())
                {
                    writer.WritePropertyName(Canvas.LabelJName);

                    writer.WriteStartArray();

                    foreach (var label in canvas.Label)
                        serializer.Serialize(writer, label);

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
            }
        }
    }
}