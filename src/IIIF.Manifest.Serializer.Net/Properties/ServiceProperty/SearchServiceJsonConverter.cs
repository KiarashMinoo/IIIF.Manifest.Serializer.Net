using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    public class SearchServiceJsonConverter : BaseItemJsonConverter<SearchService>
    {
        protected override SearchService CreateInstance(JToken element, Type objectType, SearchService existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jContext = element.TryGetToken(SearchService.ContextJName);
            if (jContext is null)
                throw new JsonNodeRequiredException<SearchService>(SearchService.ContextJName);

            var jId = element.TryGetToken(SearchService.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<SearchService>(SearchService.IdJName);

            var jProfile = element.TryGetToken(IBaseService.ProfileJName);
            if (jProfile is null)
                throw new JsonNodeRequiredException<SearchService>(IBaseService.ProfileJName);

            var service = new SearchService(jContext.ToString(), jId.ToString(), jProfile.ToString());

            var jType = element.TryGetToken(SearchService.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            return service;
        }

        protected override SearchService EnrichReadJson(SearchService service, JToken element, Type objectType, SearchService? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Handle nested services (autocomplete services)
            var jServices = element.TryGetToken(SearchService.ServiceJName);
            if (jServices != null)
            {
                if (jServices is JArray servicesArray)
                {
                    foreach (var serviceToken in servicesArray)
                    {
                        var nestedService = serviceToken.ToObject<AutoCompleteService>(serializer);
                        if (nestedService != null)
                        {
                            service.AddService(nestedService);
                        }
                    }
                }
                else if (jServices is JObject serviceObject)
                {
                    var nestedService = serviceObject.ToObject<AutoCompleteService>(serializer);
                    if (nestedService != null)
                    {
                        service.AddService(nestedService);
                    }
                }
            }

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, SearchService value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Profile))
                {
                    writer.WritePropertyName(IBaseService.ProfileJName);
                    writer.WriteValue(value.Profile);
                }

                // Serialize nested services
                if (value.Services.Any())
                {
                    writer.WritePropertyName(SearchService.ServiceJName);
                    if (value.Services.Count == 1)
                    {
                        serializer.Serialize(writer, value.Services.First());
                    }
                    else
                    {
                        writer.WriteStartArray();
                        foreach (var service in value.Services)
                        {
                            serializer.Serialize(writer, service);
                        }
                        writer.WriteEndArray();
                    }
                }
            }
        }
    }
}