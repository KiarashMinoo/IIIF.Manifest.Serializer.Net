using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    public class AutoCompleteServiceJsonConverter : BaseItemJsonConverter<AutoCompleteService>
    {
        protected override AutoCompleteService CreateInstance(JToken element, Type objectType, AutoCompleteService existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jContext = element.TryGetToken(AutoCompleteService.ContextJName);
            if (jContext is null)
                throw new JsonNodeRequiredException<AutoCompleteService>(AutoCompleteService.ContextJName);

            var jId = element.TryGetToken(AutoCompleteService.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<AutoCompleteService>(AutoCompleteService.IdJName);

            var jProfile = element.TryGetToken(IBaseService.ProfileJName);
            if (jProfile is null)
                throw new JsonNodeRequiredException<AutoCompleteService>(IBaseService.ProfileJName);

            var service = new AutoCompleteService(jContext.ToString(), jId.ToString(), jProfile.ToString());

            var jType = element.TryGetToken(AutoCompleteService.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, AutoCompleteService value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null && !string.IsNullOrEmpty(value.Profile))
            {
                writer.WritePropertyName(IBaseService.ProfileJName);
                writer.WriteValue(value.Profile);
            }
        }
    }
}