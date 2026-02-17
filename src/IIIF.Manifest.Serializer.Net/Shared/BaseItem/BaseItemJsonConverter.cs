using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.BaseItem
{
    public class BaseItemJsonConverter<TBaseItem> : TrackableObjectJsonConverter<TBaseItem>
        where TBaseItem : BaseItem<TBaseItem>
    {
        protected bool DisableTypeChecking { get; set; } = false;

        private TBaseItem SetType(JToken element, TBaseItem baseItem)
        {
            var jType = element.TryGetToken(BaseItem<TBaseItem>.TypeJName);

            if (jType != null)
                baseItem.SetType(jType.ToString());
            else if (!DisableTypeChecking)
                throw new JsonNodeRequiredException<TBaseItem>(BaseItem<TBaseItem>.TypeJName);

            return baseItem;
        }

        private TBaseItem SetService(JToken element, TBaseItem baseItem)
        {
            var jService = element.TryGetToken(BaseItem<TBaseItem>.ServiceJName);
            if (jService != null)
                baseItem.SetService(jService.ToObject<Properties.ServiceProperty.Service>());

            return baseItem;
        }
        
        protected override TBaseItem CreateInstance(JToken element, Type objectType, TBaseItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (element is JObject)
            {
                var jId = element.TryGetToken(BaseItem<TBaseItem>.IdJName);
                if (jId is null)
                    throw new JsonNodeRequiredException<TBaseItem>(BaseItem<TBaseItem>.IdJName);

                return (TBaseItem)Activator.CreateInstance(typeof(TBaseItem), jId.ToString());
            }
            else
                return (TBaseItem)Activator.CreateInstance(typeof(TBaseItem), element.Value<string>());
        }

        protected override TBaseItem EnrichReadJson(TBaseItem item, JToken element, Type objectType, TBaseItem existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = SetType(element, item);
            item = SetService(element, item);
            return item;
        }

        protected virtual void EnrichMoreWriteJson(JsonWriter writer, TBaseItem value, JsonSerializer serializer)
        {
        }

        protected sealed override void EnrichWriteJson(JsonWriter writer, TBaseItem value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteStartObject();

                if (!string.IsNullOrEmpty(value.Context))
                {
                    writer.WritePropertyName(BaseItem<TBaseItem>.ContextJName);
                    writer.WriteValue(value.Context);
                }

                if (!string.IsNullOrEmpty(value.Id))
                {
                    writer.WritePropertyName(BaseItem<TBaseItem>.IdJName);
                    writer.WriteValue(value.Id);
                }

                if (!string.IsNullOrEmpty(value.Type))
                {
                    writer.WritePropertyName(BaseItem<TBaseItem>.TypeJName);
                    writer.WriteValue(value.Type);
                }

                if (value.Service != null)
                {
                    writer.WritePropertyName(BaseItem<TBaseItem>.ServiceJName);
                    serializer.Serialize(writer, value.Service);
                }

                EnrichMoreWriteJson(writer, value, serializer);

                writer.WriteEndObject();
            }
        }
    }
}