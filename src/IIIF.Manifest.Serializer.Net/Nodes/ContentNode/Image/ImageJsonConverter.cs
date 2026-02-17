using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Content.Image
{
    public class ImageJsonConverter : BaseContentJsonConverter<Image>
    {
        protected override Image CreateInstance(JToken element, Type objectType, Image existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var id = string.Empty;
            var jId = element.TryGetToken(Image.IdJName);
            if (jId != null)
                id = jId.ToString();

            var jResource = element.TryGetToken(Image.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<Image>(Image.ResourceJName);

            var jOn = element.TryGetToken(Image.OnJName);
            if (jOn is null)
                throw new JsonNodeRequiredException<Image>(Image.OnJName);

            return new Image(id, jResource.ToObject<ImageResource>(), jOn.ToString());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Image image, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, image, serializer);

            if (image != null)
            {
                if (!string.IsNullOrEmpty(image.On))
                {
                    writer.WritePropertyName(Image.OnJName);
                    writer.WriteValue(image.On);
                }

                if (image.Resource != null)
                {
                    writer.WritePropertyName(Image.ResourceJName);
                    serializer.Serialize(writer, image.Resource);
                }

                if (!string.IsNullOrEmpty(image.Motivation))
                {
                    writer.WritePropertyName(Image.MotivationJName);
                    writer.WriteValue(image.Motivation);
                }
            }
        }
    }
}