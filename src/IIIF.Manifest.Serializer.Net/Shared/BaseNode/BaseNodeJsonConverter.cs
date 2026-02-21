using System;
using System.Linq;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.AccompanyingCanvasProperty;
using IIIF.Manifests.Serializer.Properties.DescriptionProperty;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.ProviderProperty;
using IIIF.Manifests.Serializer.Properties.RenderingProperty;
using IIIF.Manifests.Serializer.Properties.WithinProperty;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Shared.BaseNode
{
    public class BaseNodeJsonConverter<TBaseNode> : BaseItemJsonConverter<TBaseNode>
        where TBaseNode : BaseNode<TBaseNode>
    {
        protected virtual bool ShouldHandleSeeAlso(TBaseNode node) => true;
        protected virtual bool ShouldHandleWithin(TBaseNode node) => true;

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
                node.SetViewingHint(jViewingHint.ToObject<ViewingHint>());

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

        private TBaseNode SetHomepages(JToken element, TBaseNode node)
        {
            var jHomepage = element.TryGetToken(BaseNode<TBaseNode>.HomepageJName);
            if (jHomepage != null)
            {
                if (jHomepage is JArray)
                {
                    var homepages = jHomepage.ToObject<Homepage[]>();
                    foreach (var homepage in homepages)
                        node.AddHomepage(homepage);
                }
                else node.AddHomepage(jHomepage.ToObject<Homepage>());
            }

            return node;
        }

        private TBaseNode SetProviders(JToken element, TBaseNode node)
        {
            var jProvider = element.TryGetToken(BaseNode<TBaseNode>.ProviderJName);
            if (jProvider != null)
            {
                if (jProvider is JArray)
                {
                    var providers = jProvider.ToObject<Provider[]>();
                    foreach (var provider in providers)
                        node.AddProvider(provider);
                }
                else node.AddProvider(jProvider.ToObject<Provider>());
            }

            return node;
        }

        private TBaseNode SetAccompanyingCanvas(JToken element, TBaseNode node)
        {
            var jAccompanyingCanvas = element.TryGetToken(BaseNode<TBaseNode>.AccompanyingCanvasJName);
            if (jAccompanyingCanvas != null)
            {
                node.SetAccompanyingCanvas(jAccompanyingCanvas.ToObject<AccompanyingCanvas>());
            }

            return node;
        }

        private TBaseNode SetBehaviors(JToken element, TBaseNode node)
        {
            var jBehavior = element.TryGetToken(BaseNode<TBaseNode>.BehaviorJName);
            if (jBehavior != null)
            {
                if (jBehavior is JArray)
                {
                    var behaviors = jBehavior.ToObject<Behavior[]>();
                    foreach (var behavior in behaviors)
                        node.AddBehavior(behavior);
                }
                else
                {
                    // Single behavior value
                    var behavior = jBehavior.ToObject<Behavior>();
                    node.AddBehavior(behavior);
                }
            }

            return node;
        }

        private TBaseNode SetRendering(JToken element, TBaseNode node)
        {
            var jRendering = element.TryGetToken(BaseNode<TBaseNode>.RenderingJName);
            if (jRendering != null)
            {
                if (jRendering is JArray array)
                {
                    foreach (var item in array)
                    {
                        var rendering = item.ToObject<Rendering>();
                        node.AddRendering(rendering);
                    }
                }
                else
                {
                    var rendering = jRendering.ToObject<Rendering>();
                    node.AddRendering(rendering);
                }
            }

            return node;
        }

        private TBaseNode SetRelated(JToken element, TBaseNode node)
        {
            var jRelated = element.TryGetToken(BaseNode<TBaseNode>.RelatedJName);
            if (jRelated != null)
                node.SetRelated(jRelated.ToString());

            return node;
        }

        protected override TBaseNode EnrichReadJson(TBaseNode node, JToken element, Type objectType, TBaseNode? existingValue, bool hasExistingValue, JsonSerializer serializer)
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
            
            if (ShouldHandleSeeAlso(node))
                node = SetSeeAlsoes(element, node);

            node = SetHomepages(element, node);
            node = SetProviders(element, node);
            node = SetAccompanyingCanvas(element, node);
            node = SetBehaviors(element, node);

            if (ShouldHandleWithin(node))
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

                if (node.ViewingHint != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.ViewingHintJName);
                    serializer.Serialize(writer, node.ViewingHint);
                }

                if (node.Rendering != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.RenderingJName);
                    serializer.Serialize(writer, node.Rendering);
                }

                if (node.Homepage.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.HomepageJName);

                    if (node.Homepage.Count == 1)
                        serializer.Serialize(writer, node.Homepage.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var homepage in node.Homepage)
                            serializer.Serialize(writer, homepage);

                        writer.WriteEndArray();
                    }
                }

                if (node.Provider.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.ProviderJName);

                    if (node.Provider.Count == 1)
                        serializer.Serialize(writer, node.Provider.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var provider in node.Provider)
                            serializer.Serialize(writer, provider);

                        writer.WriteEndArray();
                    }
                }

                if (node.AccompanyingCanvas != null)
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.AccompanyingCanvasJName);
                    serializer.Serialize(writer, node.AccompanyingCanvas);
                }

                if (node.Behavior.Any())
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.BehaviorJName);

                    if (node.Behavior.Count == 1)
                        serializer.Serialize(writer, node.Behavior.First());
                    else
                    {
                        writer.WriteStartArray();

                        foreach (var behavior in node.Behavior)
                            serializer.Serialize(writer, behavior);

                        writer.WriteEndArray();
                    }
                }

                if (!string.IsNullOrEmpty(node.Related))
                {
                    writer.WritePropertyName(BaseNode<TBaseNode>.RelatedJName);
                    writer.WriteValue(node.Related);
                }

                if (ShouldHandleSeeAlso(node) && node.SeeAlso.Any())
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

                if (ShouldHandleWithin(node) && node.Within.Any())
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