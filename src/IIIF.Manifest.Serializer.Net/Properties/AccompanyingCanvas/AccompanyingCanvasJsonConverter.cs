using System;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using IIIF.Manifests.Serializer.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties
{
    public class AccompanyingCanvasJsonConverter : BaseItemJsonConverter<AccompanyingCanvas>
    {
        public AccompanyingCanvasJsonConverter()
        {
            DisableTypeChecking = true;
        }

        protected override AccompanyingCanvas CreateInstance(JToken element, Type objectType, AccompanyingCanvas existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (!(element is JObject))
                throw new JsonObjectMustBeJObject<AccompanyingCanvas>(nameof(AccompanyingCanvas));

            var jId = element.TryGetToken(AccompanyingCanvas.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<AccompanyingCanvas>(AccompanyingCanvas.IdJName);

            return new AccompanyingCanvas(jId.ToString());
        }
    }
}