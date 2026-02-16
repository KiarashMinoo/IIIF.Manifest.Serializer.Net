using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.Service
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

            var service = new Service(jContext.ToString(), jId.ToString(), jProfile.ToString());

            var jType = element.TryGetToken(Service.TypeJName);
            if (jType != null)
            {
                service.SetType(jType.ToString());
            }

            return service;
        }

        private Service SetTiles(JToken element, Service service)
        {
            var jTiles = element.TryGetToken(Service.TilesJName);

            if (jTiles != null && jTiles is JArray)
            {
                var tiles = jTiles.ToObject<Tile.Tile[]>();
                foreach (var tile in tiles)
                    service.AddTile(tile);
            }

            return service;
        }

        private Service SetSizes(JToken element, Service service)
        {
            var jSizes = element.TryGetToken(Service.SizesJName);

            if (jSizes != null && jSizes is JArray)
            {
                var sizes = jSizes.ToObject<Size.Size[]>();
                foreach (var size in sizes)
                    service.AddSize(size);
            }

            return service;
        }

        private Service SetMaxDimensions(JToken element, Service service)
        {
            var jMaxWidth = element.TryGetToken(Service.MaxWidthJName);
            if (jMaxWidth != null)
                service.SetMaxWidth(jMaxWidth.Value<int>());

            var jMaxHeight = element.TryGetToken(Service.MaxHeightJName);
            if (jMaxHeight != null)
                service.SetMaxHeight(jMaxHeight.Value<int>());

            var jMaxArea = element.TryGetToken(Service.MaxAreaJName);
            if (jMaxArea != null)
                service.SetMaxArea(jMaxArea.Value<long>());

            return service;
        }

        private Service SetRights(JToken element, Service service)
        {
            var jRights = element.TryGetToken(Service.RightsJName);
            if (jRights != null)
                service.SetRights(jRights.ToString());

            return service;
        }

        private Service SetPreferredFormats(JToken element, Service service)
        {
            var jFormats = element.TryGetToken(Service.PreferredFormatsJName);

            if (jFormats != null && jFormats is JArray)
            {
                foreach (var format in jFormats)
                    service.AddPreferredFormat(format.ToString());
            }

            return service;
        }

        private Service SetExtraQualities(JToken element, Service service)
        {
            var jQualities = element.TryGetToken(Service.ExtraQualitiesJName);

            if (jQualities != null && jQualities is JArray)
            {
                foreach (var quality in jQualities)
                    service.AddExtraQuality(quality.ToString());
            }

            return service;
        }

        private Service SetExtraFeatures(JToken element, Service service)
        {
            var jFeatures = element.TryGetToken(Service.ExtraFeaturesJName);

            if (jFeatures != null && jFeatures is JArray)
            {
                foreach (var feature in jFeatures)
                    service.AddExtraFeature(feature.ToString());
            }

            return service;
        }

        protected sealed override Service EnrichReadJson(Service service, JToken element, Type objectType, Service existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            service = service.SetHeight(element);
            service = service.SetWidth(element);
            service = SetTiles(element, service);
            service = SetSizes(element, service);
            service = SetMaxDimensions(element, service);
            service = SetRights(element, service);
            service = SetPreferredFormats(element, service);
            service = SetExtraQualities(element, service);
            service = SetExtraFeatures(element, service);

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

                if (value.MaxWidth != null)
                {
                    writer.WritePropertyName(Service.MaxWidthJName);
                    writer.WriteValue(value.MaxWidth.Value);
                }

                if (value.MaxHeight != null)
                {
                    writer.WritePropertyName(Service.MaxHeightJName);
                    writer.WriteValue(value.MaxHeight.Value);
                }

                if (value.MaxArea != null)
                {
                    writer.WritePropertyName(Service.MaxAreaJName);
                    writer.WriteValue(value.MaxArea.Value);
                }

                if (value.Rights != null)
                {
                    writer.WritePropertyName(Service.RightsJName);
                    writer.WriteValue(value.Rights);
                }

                if (value.Sizes.Any())
                {
                    writer.WritePropertyName(Service.SizesJName);
                    writer.WriteStartArray();
                    foreach (var size in value.Sizes)
                        serializer.Serialize(writer, size);
                    writer.WriteEndArray();
                }

                if (value.Tiles.Any())
                {
                    writer.WritePropertyName(Service.TilesJName);
                    writer.WriteStartArray();
                    foreach (var tile in value.Tiles)
                        serializer.Serialize(writer, tile);
                    writer.WriteEndArray();
                }

                if (value.PreferredFormats.Any())
                {
                    writer.WritePropertyName(Service.PreferredFormatsJName);
                    writer.WriteStartArray();
                    foreach (var format in value.PreferredFormats)
                        writer.WriteValue(format.Value);
                    writer.WriteEndArray();
                }

                if (value.ExtraQualities.Any())
                {
                    writer.WritePropertyName(Service.ExtraQualitiesJName);
                    writer.WriteStartArray();
                    foreach (var quality in value.ExtraQualities)
                        writer.WriteValue(quality.Value);
                    writer.WriteEndArray();
                }

                if (value.ExtraFeatures.Any())
                {
                    writer.WritePropertyName(Service.ExtraFeaturesJName);
                    writer.WriteStartArray();
                    foreach (var feature in value.ExtraFeatures)
                        writer.WriteValue(feature.Value);
                    writer.WriteEndArray();
                }
            }
        }
    }
}