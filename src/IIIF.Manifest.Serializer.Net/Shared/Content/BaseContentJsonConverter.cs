using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Within;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.Content
{
    public class BaseContentJsonConverter<TBaseContent> : BaseNodeJsonConverter<TBaseContent>
        where TBaseContent : BaseContent<TBaseContent>
    {
        public BaseContentJsonConverter()
        {
            DisableTypeChecking = true;
        }

        protected override bool ShouldHandleSeeAlso(TBaseContent node) => false;
        protected override bool ShouldHandleWithin(TBaseContent node) => false;

        private TBaseContent SetFormat(JToken element, TBaseContent content)
        {
            var jFormat = element.TryGetToken(BaseContent<TBaseContent>.FormatJName);
            if (jFormat != null)
                content.SetFormat(jFormat.ToString());

            return content;
        }

        private TBaseContent SetHeight(JToken element, TBaseContent content)
        {
            var jHeight = element.TryGetToken(Constants.HeightJName);
            if (jHeight != null)
                content.SetHeight(jHeight.Value<int>());

            return content;
        }

        private TBaseContent SetWidth(JToken element, TBaseContent content)
        {
            var jWidth = element.TryGetToken(Constants.WidthJName);
            if (jWidth != null)
                content.SetWidth(jWidth.Value<int>());

            return content;
        }

        private TBaseContent SetSeeAlsoes(JToken element, TBaseContent content)
        {
            var jSeeAlso = element.TryGetToken(BaseContent<TBaseContent>.SeeAlsoJName);
            if (jSeeAlso != null)
            {
                if (jSeeAlso is JArray)
                {
                    var seeAlsos = jSeeAlso.ToObject<SeeAlso[]>();
                    foreach (var seeAlso in seeAlsos)
                        content.AddSeeAlso(seeAlso);
                }
                else content.AddSeeAlso(jSeeAlso.ToObject<SeeAlso>());
            }

            return content;
        }

        private TBaseContent SetWithins(JToken element, TBaseContent content)
        {
            var jWithin = element.TryGetToken(BaseContent<TBaseContent>.WithinJName);
            if (jWithin != null)
            {
                if (jWithin is JArray)
                {
                    var withins = jWithin.ToObject<Within[]>();
                    foreach (var within in withins)
                        content.AddWithin(within);
                }
                else content.AddWithin(jWithin.ToObject<Within>());
            }

            return content;
        }

        protected override TBaseContent EnrichReadJson(TBaseContent item, JToken element, Type objectType, TBaseContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            item = base.EnrichReadJson(item, element, objectType, existingValue, hasExistingValue, serializer);

            item = SetFormat(element, item);
            item = SetHeight(element, item);
            item = SetWidth(element, item);
            item = SetSeeAlsoes(element, item);
            item = SetWithins(element, item);

            return item;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, TBaseContent content, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, content, serializer);

            if (content != null)
            {
                if (!string.IsNullOrEmpty(content.Format))
                {
                    writer.WritePropertyName(BaseContent<TBaseContent>.FormatJName);
                    writer.WriteValue(content.Format);
                }

                if (content.Height != null)
                {
                    writer.WritePropertyName(Constants.HeightJName);
                    writer.WriteValue(content.Height.Value);
                }

                if (content.Width != null)
                {
                    writer.WritePropertyName(Constants.WidthJName);
                    writer.WriteValue(content.Width.Value);
                }

                if (content.SeeAlso.Any())
                {
                    writer.WritePropertyName(BaseContent<TBaseContent>.SeeAlsoJName);

                    writer.WriteStartArray();

                    foreach (var seeAlso in content.SeeAlso)
                        serializer.Serialize(writer, seeAlso);

                    writer.WriteEndArray();
                }

                if (content.Within.Any())
                {
                    writer.WritePropertyName(BaseContent<TBaseContent>.WithinJName);

                    writer.WriteStartArray();

                    foreach (var within in content.Within)
                        serializer.Serialize(writer, within);

                    writer.WriteEndArray();
                }
            }
        }
    }

    public class BaseContentJsonConverter<TContent, TResource> : BaseContentJsonConverter<TContent>
        where TContent : BaseContent<TContent, TResource>
        where TResource : BaseResource<TResource>
    {
        protected override TContent CreateInstance(JToken element, Type objectType, TContent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jId = element.TryGetToken(BaseContent<TContent, TResource>.IdJName);
            if (jId is null)
                throw new JsonNodeRequiredException<TContent>(BaseContent<TContent, TResource>.IdJName);

            var jType = element.TryGetToken(BaseContent<TContent, TResource>.TypeJName);
            if (jType is null)
                throw new JsonNodeRequiredException<TContent>(BaseContent<TContent, TResource>.TypeJName);

            var jResource = element.TryGetToken(BaseContent<TContent, TResource>.ResourceJName);
            if (jResource is null)
                throw new JsonNodeRequiredException<TContent>(BaseContent<TContent, TResource>.ResourceJName);

            return (TContent)Activator.CreateInstance(typeof(TContent), jId.ToString(), jType.ToString(), jResource.ToObject<TResource>());
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, TContent content, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, content, serializer);

            if (content != null)
            {
                if (content.Resource != null)
                {
                    writer.WritePropertyName(BaseContent<TContent, TResource>.ResourceJName);
                    serializer.Serialize(writer, content.Resource);
                }
            }
        }
    }
}