using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource
{
    public class ImageResourceJsonConverter : BaseResourceJsonConverter<ImageResource>
    {
        private ImageResource SetHeight(JToken element, ImageResource resource)
        {
            var jHeight = element.TryGetToken(Constants.HeightJName);
            if (jHeight != null)
                resource.SetHeight(jHeight.Value<int>());

            return resource;
        }

        private ImageResource SetWidth(JToken element, ImageResource resource)
        {
            var jWidth = element.TryGetToken(Constants.WidthJName);
            if (jWidth != null)
                resource.SetWidth(jWidth.Value<int>());

            return resource;
        }

        protected override ImageResource CreateInstance(JToken element, Type objectType, ImageResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(ImageResource.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<ImageResource>(ImageResource.IdJName);

            var jFormat = element.TryGetToken(ImageResource.FormatJName);
            if (jFormat is null)
                throw new JsonNodeRequiredException<ImageResource>(ImageResource.FormatJName);

            return new ImageResource(jId.ToString(), jFormat.ToString());
        }
        protected override ImageResource EnrichReadJson(ImageResource resource, JToken element, Type objectType, ImageResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            resource = base.EnrichReadJson(resource, element, objectType, existingValue, hasExistingValue, serializer);

            resource = SetHeight(element, resource);
            resource = SetWidth(element, resource);

            return resource;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, ImageResource imageResource, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, imageResource, serializer);

            if (imageResource != null)
            {
                if (imageResource.Height != null)
                {
                    writer.WritePropertyName(Constants.HeightJName);
                    writer.WriteValue(imageResource.Height.Value);
                }

                if (imageResource.Width != null)
                {
                    writer.WritePropertyName(Constants.WidthJName);
                    writer.WriteValue(imageResource.Width.Value);
                }
            }
        }
    }
}