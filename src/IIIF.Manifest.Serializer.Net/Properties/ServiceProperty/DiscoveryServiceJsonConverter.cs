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
    public class DiscoveryServiceJsonConverter : BaseItemJsonConverter<DiscoveryService>
    {
        protected override DiscoveryService CreateInstance(JToken element, Type objectType, DiscoveryService existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jContext = element.TryGetToken(DiscoveryService.ContextJName);
            if (jContext is null)
                throw new JsonNodeRequiredException<DiscoveryService>(DiscoveryService.ContextJName);

            var jId = element.TryGetToken(DiscoveryService.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<DiscoveryService>(DiscoveryService.IdJName);

            var jProfile = element.TryGetToken(IBaseService.ProfileJName);
            if (jProfile is null)
                throw new JsonNodeRequiredException<DiscoveryService>(IBaseService.ProfileJName);

            var service = new DiscoveryService(jContext.ToString(), jId.ToString(), jProfile.ToString());

            var jType = element.TryGetToken(DiscoveryService.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            return service;
        }

        protected override DiscoveryService EnrichReadJson(DiscoveryService service, JToken element, Type objectType, DiscoveryService existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Handle ordered items (activities)
            var jOrderedItems = element.TryGetToken(DiscoveryService.OrderedItemsJName);
            if (jOrderedItems != null && jOrderedItems is JArray activitiesArray)
            {
                foreach (var activityToken in activitiesArray)
                {
                    var activity = activityToken.ToObject<Activity>(serializer);
                    if (activity != null)
                    {
                        service.AddActivity(activity);
                    }
                }
            }

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, DiscoveryService value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Profile))
                {
                    writer.WritePropertyName(IBaseService.ProfileJName);
                    writer.WriteValue(value.Profile);
                }

                // Serialize ordered items (activities)
                if (value.OrderedItems.Any())
                {
                    writer.WritePropertyName(DiscoveryService.OrderedItemsJName);
                    writer.WriteStartArray();
                    foreach (var activity in value.OrderedItems)
                    {
                        serializer.Serialize(writer, activity);
                    }
                    writer.WriteEndArray();
                }
            }
        }
    }
}