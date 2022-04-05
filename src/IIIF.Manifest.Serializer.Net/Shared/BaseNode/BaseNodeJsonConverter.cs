using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace IIIF.Manifests.Serializer.Shared
{
    public class BaseNodeJsonConverter<TBaseNode> : BaseItemJsonConverter<TBaseNode>
        where TBaseNode : BaseNode<TBaseNode>
    {
        private TBaseNode SetLabels(JToken element, TBaseNode node)
        {
            var jLabel = element.TryGetToken(BaseNode<TBaseNode>.LabelJName);

            if (jLabel != null)
            {
                if (jLabel is JArray)
                {
                    var labels = jLabel.ToObject<Label[]>();
                    foreach (var label in labels)
                        node.AddLabel(label);
                }
                else
                    node.AddLabel(jLabel.ToObject<Label>());
            }

            return node;
        }

        private TBaseNode SetDescriptions(JToken element, TBaseNode node)
        {
            var jDescription = element.TryGetToken(BaseNode<TBaseNode>.DescriptionJName);
            if (jDescription != null)
            {
                if (jDescription is JArray)
                {
                    var descriptions = jDescription.ToObject<Description[]>();
                    foreach (var description in descriptions)
                        node.AddDescription(description);
                }
                else
                    node.AddDescription(jDescription.ToObject<Description>());
            }

            return node;
        }

        private TBaseNode SetMetadatas(JToken element, TBaseNode node)
        {
            var jMetadata = element.TryGetToken(BaseNode<TBaseNode>.MetadataJName);
            if (jMetadata != null)
            {
                if (jMetadata is JArray)
                {
                    var metadatas = jMetadata.ToObject<Metadata[]>();
                    foreach (var metadata in metadatas)
                        node.AddMetadata(metadata);
                }
                else node.AddMetadata(jMetadata.ToObject<Metadata>());
            }

            return node;
        }

        private TBaseNode SetAttributions(JToken element, TBaseNode node)
        {
            var jAttribution = element.TryGetToken(BaseNode<TBaseNode>.AttributionJName);
            if (jAttribution != null)
            {
                if (jAttribution is JArray)
                {
                    var attributions = jAttribution.ToObject<Attribution[]>();
                    foreach (var attribution in attributions)
                        node.AddAttribution(attribution);
                }
                else node.AddAttribution(jAttribution.ToObject<Attribution>());
            }

            return node;
        }

        private TBaseNode SetLogo(JToken element, TBaseNode node)
        {
            var jLogo = element.TryGetToken(BaseNode<TBaseNode>.LogoJName);
            if (jLogo != null)
                node.SetLogo(jLogo.ToObject<Logo>());

            return node;
        }

        private TBaseNode SetThumbnail(JToken element, TBaseNode node)
        {
            var jThumbnail = element.TryGetToken(BaseNode<TBaseNode>.ThumbnailJName);
            if (jThumbnail != null)
                node.SetThumbnail(jThumbnail.ToObject<Thumbnail>());

            return node;
        }

        private TBaseNode SetLicense(JToken element, TBaseNode node)
        {
            var jLicense = element.TryGetToken(BaseNode<TBaseNode>.LicenseJName);
            if (jLicense != null)
                node.SetLicense(jLicense.ToObject<License>());

            return node;
        }

        private TBaseNode SetViewingHint(JToken element, TBaseNode node)
        {
            var jViewingHint = element.TryGetToken(BaseNode<TBaseNode>.ViewingHintJName);
            if (jViewingHint != null)
                node.SetViewingHint(jViewingHint.ToString());

            return node;
        }

        private TBaseNode SetSeeAlsoes(JToken element, TBaseNode node)
        {
            var jSeeAlso = element.TryGetToken(BaseNode<TBaseNode>.SeeAlsoJName);
            if (jSeeAlso != null)
            {
                if (jSeeAlso is JArray)
                {
                    var seeAlsos = jSeeAlso.ToObject<SeeAlso[]>();
                    foreach (var seeAlso in seeAlsos)
                        node.AddSeeAlso(seeAlso);
                }
                else node.AddSeeAlso(jSeeAlso.ToObject<SeeAlso>());
            }

            return node;
        }

        private TBaseNode SetWithins(JToken element, TBaseNode node)
        {
            var jWithin = element.TryGetToken(BaseNode<TBaseNode>.WithinJName);
            if (jWithin != null)
            {
                if (jWithin is JArray)
                {
                    var withins = jWithin.ToObject<Within[]>();
                    foreach (var within in withins)
                        node.AddWithin(within);
                }
                else node.AddWithin(jWithin.ToObject<Within>());
            }

            return node;
        }

        private TBaseNode SetRendering(JToken element, TBaseNode node)
        {
            var jRendering = element.TryGetToken(BaseNode<TBaseNode>.RenderingJName);
            if (jRendering != null)
                node.SetRendering(jRendering.ToObject<Rendering>());

            return node;
        }

        private TBaseNode SetRelated(JToken element, TBaseNode node)
        {
            var jRelated = element.TryGetToken(BaseNode<TBaseNode>.RelatedJName);
            if (jRelated != null)
                node.SetRelated(jRelated.ToString());

            return node;
        }

        protected override TBaseNode EnrichReadJson(TBaseNode node, JToken element, Type objectType, TBaseNode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            node = base.EnrichReadJson(node, element, objectType, existingValue, hasExistingValue, serializer);

            node = SetLabels(element, node);
            node = SetDescriptions(element, node);
            node = SetMetadatas(element, node);
            node = SetAttributions(element, node);
            node = SetLogo(element, node);
            node = SetThumbnail(element, node);
            node = SetLicense(element, node);
            node = SetViewingHint(element, node);
            node = SetRendering(element, node);
            node = SetRelated(element, node);
            node = SetSeeAlsoes(element, node);
            node = SetWithins(element, node);

            return node;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, TBaseNode node, JsonSerializer serializer)
        {
            base.EnrichMoreWriteJson(writer, node, serializer);

            if (node != null)
            {
                if (node.Label.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.LabelJName);

                    if (node.Label.Count == 1)
                        serializer.Serialize(writer, node.Label.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var label in node.Label)
                            serializer.Serialize(writer, label);

                        writer.WriteEndArray();
                    }
                }

                if (node.Description.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.DescriptionJName);

                    if (node.Description.Count == 1)
                        serializer.Serialize(writer, node.Description.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var description in node.Description)
                            serializer.Serialize(writer, description);

                        writer.WriteEndArray();
                    }
                }

                if (node.Metadata.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.MetadataJName);

                    writer.WriteStartArray();

                    foreach (var metadata in node.Metadata)
                        serializer.Serialize(writer, metadata);

                    writer.WriteEndArray();
                }

                if (node.Attribution.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.AttributionJName);

                    if (node.Attribution.Count == 1)
                        serializer.Serialize(writer, node.Attribution.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var attribution in node.Attribution)
                            serializer.Serialize(writer, attribution);

                        writer.WriteEndArray();
                    }
                }

                if (node.Logo != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.LogoJName);
                    serializer.Serialize(writer, node.Logo);
                }

                if (node.Thumbnail != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.ThumbnailJName);
                    serializer.Serialize(writer, node.Thumbnail);
                }

                if (node.License != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.LicenseJName);
                    serializer.Serialize(writer, node.License);
                }

                if (!string.IsNullOrEmpty(node.ViewingHint))
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.ViewingHintJName);
                    writer.WriteValue(node.ViewingHint);
                }

                if (node.Rendering != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.RenderingJName);
                    serializer.Serialize(writer, node.Rendering);
                }

                if (!string.IsNullOrEmpty(node.Related))
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.RelatedJName);
                    writer.WriteValue(node.Related);
                }

                if (node.SeeAlso.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.SeeAlsoJName);

                    if (node.SeeAlso.Count == 1)
                        serializer.Serialize(writer, node.SeeAlso.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var seeAlso in node.SeeAlso)
                            serializer.Serialize(writer, seeAlso);

                        writer.WriteEndArray();
                    }
                }

                if (node.Within.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.WithinJName);

                    if (node.Within.Count == 1)
                        serializer.Serialize(writer, node.Within.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var within in node.Within)
                            serializer.Serialize(writer, within);

                        writer.WriteEndArray();
                    }
                }
            }
        }
    }
}