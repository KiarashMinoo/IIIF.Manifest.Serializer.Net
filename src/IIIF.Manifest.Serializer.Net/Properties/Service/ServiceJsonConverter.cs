using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace IIIF.Manifests.Serializer.Properties
{
    public class ServiceJsonConverter : BaseItemJsonConverter<Service>
    {
        protected override Service CreateInstance(JToken element, Type objectType, Service existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jContext = element.TryGetToken(Service.ContextJName);
            if (jContext is null)
                throw new JsonNodeRequiredException<Service>(Service.ContextJName);

            var jId = element.TryGetToken(Service.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<Service>(Service.IdJName);

            var jProfile = element.TryGetToken(Service.ProfileJName);
            if (jProfile is null)
                throw new JsonNodeRequiredException<Service>(Service.ProfileJName);

            return new Service(jContext.ToString(), jId.ToString(), jProfile.ToString());
        }

        private Service SetTiles(JToken element, Service service)
        {
            var jTiles = element.TryGetToken(Service.TilesJName);

            if (jTiles != null && jTiles is JArray)
            {
                var tiles = jTiles.ToObject<Tile[]>();
                foreach (var tile in tiles)
                    service.AddTile(tile);

            }

            return service;
        }

        protected sealed override Service EnrichReadJson(Service service, JToken element, Type objectType, Service existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            service = service.SetHeight(element);
            service = service.SetWidth(element);
            service = SetTiles(element, service);

            return service;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Service value, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, value, serializer);

            if (value != null)
            {
                if (!string.IsNullOrEmpty(value.Profile))
                {
                    writer.WritePropertyName(Service.ProfileJName);
                    writer.WriteValue(value.Profile);
                }

                if (value.Height != null)
                {
                    writer.WritePropertyName(Constants.HeightJName);
                    writer.WriteValue(value.Height.Value);
                }

                if (value.Width != null)
                {
                    writer.WritePropertyName(Constants.WidthJName);
                    writer.WriteValue(value.Width.Value);
                }

                if (value.Tiles.Any())
                {
                    writer.WritePropertyName(Service.TilesJName);

                    writer.WriteStartArray();

                    foreach (var tile in value.Tiles)
                        serializer.Serialize(writer, tile);

                    writer.WriteEndArray();
                }
            }
        }
    }
}