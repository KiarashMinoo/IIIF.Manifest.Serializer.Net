using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Helpers
{
    public static class JsonHelper
    {
        public static JToken TryGetToken(this JToken element, string propertyName)
        {
            return element is JObject jObject && jObject.TryGetValue(propertyName, out JToken rtn) ? rtn : null;
        }
    }
}