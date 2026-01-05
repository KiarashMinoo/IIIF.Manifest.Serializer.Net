using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Trackable
{
    public class TrackableObjectJsonConverter<TTrackableObject> : JsonConverter<TTrackableObject>
        where TTrackableObject : TrackableObject<TTrackableObject>
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        protected virtual TTrackableObject CreateInstance(JToken element, Type objectType, TTrackableObject existingValue, bool hasExistingValue, JsonSerializer serializer) => null;

        protected virtual TTrackableObject EnrichReadJson(TTrackableObject item, JToken element, Type objectType, TTrackableObject existingValue, bool hasExistingValue, JsonSerializer serializer) => item;
        public sealed override TTrackableObject ReadJson(JsonReader reader, Type objectType, TTrackableObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var element = JToken.Load(reader);
            var item = CreateInstance(element, objectType, existingValue, hasExistingValue, serializer) ?? Activator.CreateInstance<TTrackableObject>();
            var rtn = EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);
            rtn?.ClearModifiedProperties();
            return rtn;
        }

        protected virtual void EnrichWriteJson(JsonWriter writer, TTrackableObject value, JsonSerializer serializer) { }
        public sealed override void WriteJson(JsonWriter writer, TTrackableObject value, JsonSerializer serializer)
        {
            writer.Formatting = Formatting.Indented;

            EnrichWriteJson(writer, value, serializer);
        }
    }
}