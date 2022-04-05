using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace IIIF.Manifests.Serializer.Nodes
{
    public class OtherContentJsonConverter : BaseContentJsonConverter<OtherContent>
    {
        protected override OtherContent CreateInstance(JToken element, Type objectType, OtherContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return base.CreateInstance(element, objectType, existingValue, hasExistingValue, serializer);
        }
    }
}