using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Service
{
    public class AuthService2JsonConverter : BaseItemJsonConverter<AuthService2>
    {
        protected override AuthService2 CreateInstance(JToken element, Type objectType, AuthService2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(AuthService2.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<AuthService2>(AuthService2.IdJName);

            var service = new AuthService2(jId.ToString());

            var jType = element.TryGetToken(AuthService2.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            var jProfile = element.TryGetToken(AuthService2.ProfileJName);
            if (jProfile != null)
            {
                service.SetProfile(jProfile.ToString());
            }

            return service;
        }

        private AuthService2 SetTextProperties(JToken element, AuthService2 service)
        {
            var jLabel = element.TryGetToken(AuthService2.LabelJName);
            if (jLabel != null)
                service.SetLabel(jLabel.ToString());

            var jHeading = element.TryGetToken(AuthService2.HeadingJName);
            if (jHeading != null)
                service.SetHeading(jHeading.ToString());

            var jNote = element.TryGetToken(AuthService2.NoteJName);
            if (jNote != null)
                service.SetNote(jNote.ToString());

            var jConfirmLabel = element.TryGetToken(AuthService2.ConfirmLabelJName);
            if (jConfirmLabel != null)
                service.SetConfirmLabel(jConfirmLabel.ToString());

            return service;
        }

        private AuthService2 SetServices(JToken element, AuthService2 service, JsonSerializer serializer)
        {
            var jServices = element.TryGetToken(AuthService2.ServiceJName);

            if (jServices != null)
            {
                if (jServices is JArray servicesArray)
                {
                    var services = servicesArray.ToObject<AuthService2[]>(serializer);
                    foreach (var svc in services)
                        service.AddService(svc);
                }
                else if (jServices is JObject)
                {
                    var svc = jServices.ToObject<AuthService2>(serializer);
                    service.AddService(svc);
                }
            }

            return service;
        }

        protected sealed override AuthService2 EnrichReadJson(AuthService2 service, JToken element, Type objectType, AuthService2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            service = SetTextProperties(element, service);
            service = SetServices(element, service, serializer);

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, AuthService2 value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Profile))
                {
                    writer.WritePropertyName(AuthService2.ProfileJName);
                    writer.WriteValue(value.Profile);
                }

                if (!string.IsNullOrEmpty(value.Label))
                {
                    writer.WritePropertyName(AuthService2.LabelJName);
                    writer.WriteValue(value.Label);
                }

                if (!string.IsNullOrEmpty(value.Heading))
                {
                    writer.WritePropertyName(AuthService2.HeadingJName);
                    writer.WriteValue(value.Heading);
                }

                if (!string.IsNullOrEmpty(value.Note))
                {
                    writer.WritePropertyName(AuthService2.NoteJName);
                    writer.WriteValue(value.Note);
                }

                if (!string.IsNullOrEmpty(value.ConfirmLabel))
                {
                    writer.WritePropertyName(AuthService2.ConfirmLabelJName);
                    writer.WriteValue(value.ConfirmLabel);
                }

                if (value.Services.Any())
                {
                    writer.WritePropertyName(AuthService2.ServiceJName);
                    if (value.Services.Count == 1)
                    {
                        serializer.Serialize(writer, value.Services.First());
                    }
                    else
                    {
                        writer.WriteStartArray();
                        foreach (var service in value.Services)
                            serializer.Serialize(writer, service);
                        writer.WriteEndArray();
                    }
                }
            }
        }
    }
}
