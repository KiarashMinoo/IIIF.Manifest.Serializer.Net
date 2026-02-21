using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Trackable
{
    public class TrackableObjectJsonConverter<TTrackableObject> : JsonConverter<TTrackableObject>
        where TTrackableObject : TrackableObject<TTrackableObject>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        protected virtual TTrackableObject? CreateInstance(
            JToken element,
            Type objectType,
            TTrackableObject? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        ) => null;

        //Read

        protected virtual TTrackableObject EnrichReadJson(TTrackableObject item,
            JToken element,
            Type objectType,
            TTrackableObject? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            return item;
        }

        public sealed override TTrackableObject ReadJson(
            JsonReader reader,
            Type objectType,
            TTrackableObject? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            var element = JToken.Load(reader);
            var item = CreateInstance(element, objectType, existingValue, hasExistingValue, serializer) ?? Activator.CreateInstance<TTrackableObject>();
            var rtn = EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);

            if (element is JObject jObject)
            {
                foreach (var property in jObject.Properties().Where(x => !rtn.ElementDescriptors.ContainsKey(x.Name)))
                {
                    rtn.SetAdditionalElementValue(property.Name, property.Value.ToString(Formatting.None));
                }
            }

            return rtn;
        }

        //Write

        protected virtual void EnrichWriteJson(JsonWriter writer, TTrackableObject value, JsonSerializer serializer)
        {
        }

        public sealed override void WriteJson(JsonWriter writer, TTrackableObject? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();

            EnrichWriteJson(writer, value, serializer);

            foreach (var (name, elementDescriptor) in value.ElementDescriptors.Where(x => x.Value.IsAdditional))
            {
                writer.WritePropertyName(name);
                writer.WriteRawValue(elementDescriptor.Value.ToString());
            }

            writer.WriteEndObject();
        }
    }
}