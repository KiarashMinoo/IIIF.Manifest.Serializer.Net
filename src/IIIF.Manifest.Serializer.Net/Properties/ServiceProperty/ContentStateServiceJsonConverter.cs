using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    public class ContentStateServiceJsonConverter : BaseItemJsonConverter<ContentStateService>
    {
        protected override ContentStateService CreateInstance(JToken element, Type objectType, ContentStateService existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jContext = element.TryGetToken(ContentStateService.ContextJName);
            if (jContext is null)
                throw new JsonNodeRequiredException<ContentStateService>(ContentStateService.ContextJName);

            var jId = element.TryGetToken(ContentStateService.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<ContentStateService>(ContentStateService.IdJName);

            var jProfile = element.TryGetToken(IBaseService.ProfileJName);
            if (jProfile is null)
                throw new JsonNodeRequiredException<ContentStateService>(IBaseService.ProfileJName);

            var service = new ContentStateService(jContext.ToString(), jId.ToString(), jProfile.ToString());

            var jType = element.TryGetToken(ContentStateService.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, ContentStateService value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (!string.IsNullOrEmpty(value.Profile))
            {
                writer.WritePropertyName(IBaseService.ProfileJName);
                writer.WriteValue(value.Profile);
            }
        }
    }
}