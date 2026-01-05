using System;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Content.OtherContent
{
    public class OtherContentJsonConverter : BaseContentJsonConverter<OtherContent>
    {
        protected override OtherContent CreateInstance(JToken element, Type objectType, OtherContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return base.CreateInstance(element, objectType, existingValue, hasExistingValue, serializer);
        }
    }
}