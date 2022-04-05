using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IIIF.Manifests.Serializer.Nodes
{
    public class EmbeddedContentJsonConverter : BaseContentJsonConverter<EmbeddedContent>
    {
        protected override EmbeddedContent CreateInstance(JToken element, Type objectType, EmbeddedContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(Image.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Image>(Image.IdJName);

            var jResource = element.TryGetToken(Image.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<Image>(Image.ResourceJName);

            var jOn = element.TryGetToken(Image.OnJName);
            if (jOn is null)
                throw new JsonNodeRequiredException<Image>(Image.OnJName);

            return new EmbeddedContent(jId.ToString(), jResource.ToObject<EmbeddedContentResource>(), jOn.ToString());
        }
    }
}