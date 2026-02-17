using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Content.Embedded.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Content.Embedded
{
    public class EmbeddedContentJsonConverter : BaseContentJsonConverter<EmbeddedContent>
    {
        protected override EmbeddedContent CreateInstance(JToken element, Type objectType, EmbeddedContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Image.Image.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Image.Image>(Image.Image.IdJName);

            var jResource = element.TryGetToken(Image.Image.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<Image.Image>(Image.Image.ResourceJName);

            var jOn = element.TryGetToken(Image.Image.OnJName);
            if (jOn is null)
                throw new JsonNodeRequiredException<Image.Image>(Image.Image.OnJName);

            return new EmbeddedContent(jId.ToString(), jResource.ToObject<EmbeddedContentResource>(), jOn.ToString());
        }
    }
}