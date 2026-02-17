using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    public class AuthService1JsonConverter : BaseItemJsonConverter<AuthService1>
    {
        protected override AuthService1 CreateInstance(JToken element, Type objectType, AuthService1 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(AuthService1.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<AuthService1>(AuthService1.IdJName);

            var jProfile = element.TryGetToken(AuthService1.ProfileJName);
            if (jProfile is null)
                throw new JsonNodeRequiredException<AuthService1>(AuthService1.ProfileJName);

            var service = new AuthService1(jId.ToString(), jProfile.ToString());

            var jType = element.TryGetToken(AuthService1.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            return service;
        }

        private AuthService1 SetTextProperties(JToken element, AuthService1 service)
        {
            var jLabel = element.TryGetToken(AuthService1.LabelJName);
            if (jLabel != null)
                service.SetLabel(jLabel.ToString());

            var jHeader = element.TryGetToken(AuthService1.HeaderJName);
            if (jHeader != null)
                service.SetHeader(jHeader.ToString());

            var jDescription = element.TryGetToken(AuthService1.DescriptionJName);
            if (jDescription != null)
                service.SetDescription(jDescription.ToString());

            var jConfirmLabel = element.TryGetToken(AuthService1.ConfirmLabelJName);
            if (jConfirmLabel != null)
                service.SetConfirmLabel(jConfirmLabel.ToString());

            var jFailureHeader = element.TryGetToken(AuthService1.FailureHeaderJName);
            if (jFailureHeader != null)
                service.SetFailureHeader(jFailureHeader.ToString());

            var jFailureDescription = element.TryGetToken(AuthService1.FailureDescriptionJName);
            if (jFailureDescription != null)
                service.SetFailureDescription(jFailureDescription.ToString());

            return service;
        }

        private AuthService1 SetServices(JToken element, AuthService1 service, JsonSerializer serializer)
        {
            var jServices = element.TryGetToken(AuthService1.ServiceJName);

            if (jServices != null)
            {
                if (jServices is JArray servicesArray)
                {
                    var services = servicesArray.ToObject<AuthService1[]>(serializer);
                    foreach (var svc in services)
                        service.AddService(svc);
                }
                else if (jServices is JObject)
                {
                    var svc = jServices.ToObject<AuthService1>(serializer);
                    service.AddService(svc);
                }
            }

            return service;
        }

        protected sealed override AuthService1 EnrichReadJson(AuthService1 service, JToken element, Type objectType, AuthService1 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            service = SetTextProperties(element, service);
            service = SetServices(element, service, serializer);

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, AuthService1 value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Profile))
                {
                    writer.WritePropertyName(AuthService1.ProfileJName);
                    writer.WriteValue(value.Profile);
                }

                if (!string.IsNullOrEmpty(value.Label))
                {
                    writer.WritePropertyName(AuthService1.LabelJName);
                    writer.WriteValue(value.Label);
                }

                if (!string.IsNullOrEmpty(value.Header))
                {
                    writer.WritePropertyName(AuthService1.HeaderJName);
                    writer.WriteValue(value.Header);
                }

                if (!string.IsNullOrEmpty(value.Description))
                {
                    writer.WritePropertyName(AuthService1.DescriptionJName);
                    writer.WriteValue(value.Description);
                }

                if (!string.IsNullOrEmpty(value.ConfirmLabel))
                {
                    writer.WritePropertyName(AuthService1.ConfirmLabelJName);
                    writer.WriteValue(value.ConfirmLabel);
                }

                if (!string.IsNullOrEmpty(value.FailureHeader))
                {
                    writer.WritePropertyName(AuthService1.FailureHeaderJName);
                    writer.WriteValue(value.FailureHeader);
                }

                if (!string.IsNullOrEmpty(value.FailureDescription))
                {
                    writer.WritePropertyName(AuthService1.FailureDescriptionJName);
                    writer.WriteValue(value.FailureDescription);
                }

                if (value.Services.Any())
                {
                    writer.WritePropertyName(AuthService1.ServiceJName);
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
