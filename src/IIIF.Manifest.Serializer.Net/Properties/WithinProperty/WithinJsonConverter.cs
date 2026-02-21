using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Properties.WithinProperty
{
    public class WithinJsonConverter : BaseItemJsonConverter<Within>
    {
        private Within SetLabel(JToken element, Within within)
        {
            var jLabel = element.TryGetToken(Within.LabelJName);
            if (jLabel != null)
                within.SetLabel(jLabel.ToString());

            return within;
        }

        public WithinJsonConverter() => DisableTypeChecking = true;

        protected override Within CreateInstance(JToken element, Type objectType, Within existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (element is JObject)
                return new Within(element.TryGetToken("@id").ToString());

            return base.CreateInstance(element, objectType, existingValue, hasExistingValue, serializer);
        }

        protected override Within EnrichReadJson(Within item, JToken element, Type objectType, Within? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = base.EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);

            SetLabel(element, item);

            return item;
        }
    }
}