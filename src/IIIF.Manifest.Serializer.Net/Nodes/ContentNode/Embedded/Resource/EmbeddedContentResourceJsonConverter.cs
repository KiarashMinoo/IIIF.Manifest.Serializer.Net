using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Embedded.Resource
{
    public class EmbeddedContentResourceJsonConverter : BaseResourceJsonConverter<EmbeddedContentResource>
    {
        protected override EmbeddedContentResource CreateInstance(JToken element, Type objectType, EmbeddedContentResource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jChars = element.TryGetToken(EmbeddedContentResource.CharsJName);
            if (jChars is null)
                throw new JsonNodeRequiredException<EmbeddedContentResource>(EmbeddedContentResource.CharsJName);

            var jLanguage = element.TryGetToken(EmbeddedContentResource.LanguageJname);
            return new EmbeddedContentResource(jChars.ToString(), jLanguage?.ToString());
        }
    }
}