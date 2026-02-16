using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Nodes.Collection
{
    public class CollectionJsonConverter : BaseNodeJsonConverter<Collection>
    {
        private Collection ConstructCollection(JToken element)
        {
            var jId = element.TryGetToken(Collection.IdJName);
            if (jId is null)
                throw new ArgumentException("Collection requires @id");

            var jLabel = element.TryGetToken(Collection.LabelJName);
            if (jLabel != null)
            {
                if (jLabel is JArray labelArray)
                {
                    var labels = labelArray.ToObject<Label[]>();
                    return new Collection(jId.ToString(), labels);
                }
                else
                {
                    return new Collection(jId.ToString(), new Label(jLabel.ToString()));
                }
            }

            return new Collection(jId.ToString()) { };
        }

        private Collection SetCollections(JToken element, Collection collection)
        {
            var jCollections = element.TryGetToken(Collection.CollectionsJName);
            if (jCollections is JArray array)
            {
                var collections = array.ToObject<Collection[]>();
                foreach (var c in collections)
                    collection.AddCollection(c);
            }

            return collection;
        }

        private Collection SetManifests(JToken element, Collection collection)
        {
            var jManifests = element.TryGetToken(Collection.ManifestsJName);
            if (jManifests is JArray array)
            {
                foreach (var m in array)
                {
                    var id = m["@id"]?.ToString();
                    if (id != null)
                        collection.AddManifest(id);
                }
            }

            return collection;
        }

        private Collection SetTotal(JToken element, Collection collection)
        {
            var jTotal = element.TryGetToken(Collection.TotalJName);
            if (jTotal != null && int.TryParse(jTotal.ToString(), out var total))
            {
                collection.SetTotal(total);
            }

            return collection;
        }

        private Collection SetPaging(JToken element, Collection collection)
        {
            var jFirst = element.TryGetToken(Collection.FirstJName);
            if (jFirst != null)
                collection.SetFirst(jFirst.ToString());

            var jLast = element.TryGetToken(Collection.LastJName);
            if (jLast != null)
                collection.SetLast(jLast.ToString());

            var jNext = element.TryGetToken(Collection.NextJName);
            if (jNext != null)
                collection.SetNext(jNext.ToString());

            var jPrev = element.TryGetToken(Collection.PrevJName);
            if (jPrev != null)
                collection.SetPrev(jPrev.ToString());

            var jStartIndex = element.TryGetToken(Collection.StartIndexJName);
            if (jStartIndex != null && int.TryParse(jStartIndex.ToString(), out var startIndex))
                collection.SetStartIndex(startIndex);

            return collection;
        }

        private Collection SetMembers(JToken element, Collection collection)
        {
            var jMembers = element.TryGetToken(Collection.MembersJName);
            if (jMembers is JArray array)
            {
                foreach (var m in array)
                    collection.AddMember(m.ToObject<object>());
            }

            return collection;
        }

        protected override Collection CreateInstance(JToken element, Type objectType, Collection existingValue, bool hasExistingValue, JsonSerializer serializer) 
            => ConstructCollection(element);

        protected override Collection EnrichReadJson(Collection collection, JToken element, Type objectType, Collection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            SetCollections(element, collection);
            SetManifests(element, collection);
            SetMembers(element, collection);
            SetTotal(element, collection);
            SetPaging(element, collection);
            collection.SetViewingDirection(element);

            return collection;
        }

        protected override void EnrichMoreWriteJson(JsonWriter writer, Collection collection, JsonSerializer serializer)
        {
            if (collection.ViewingDirection != null)
            {
                writer.WritePropertyName(Constants.ViewingDirectionJName);
                serializer.Serialize(writer, collection.ViewingDirection);
            }

            if (collection.Collections.Count > 0)
            {
                writer.WritePropertyName(Collection.CollectionsJName);
                serializer.Serialize(writer, collection.Collections);
            }

            if (collection.Manifests.Count > 0)
            {
                writer.WritePropertyName(Collection.ManifestsJName);
                writer.WriteStartArray();
                foreach (var manifestId in collection.Manifests)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("@id");
                    writer.WriteValue(manifestId);
                    writer.WritePropertyName("@type");
                    writer.WriteValue("sc:Manifest");
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
            }

            if (collection.Total.HasValue)
            {
                writer.WritePropertyName(Collection.TotalJName);
                writer.WriteValue(collection.Total.Value);
            }

            if (!string.IsNullOrEmpty(collection.First))
            {
                writer.WritePropertyName(Collection.FirstJName);
                writer.WriteValue(collection.First);
            }

            if (!string.IsNullOrEmpty(collection.Last))
            {
                writer.WritePropertyName(Collection.LastJName);
                writer.WriteValue(collection.Last);
            }

            if (!string.IsNullOrEmpty(collection.Next))
            {
                writer.WritePropertyName(Collection.NextJName);
                writer.WriteValue(collection.Next);
            }

            if (!string.IsNullOrEmpty(collection.Prev))
            {
                writer.WritePropertyName(Collection.PrevJName);
                writer.WriteValue(collection.Prev);
            }

            if (collection.StartIndex.HasValue)
            {
                writer.WritePropertyName(Collection.StartIndexJName);
                writer.WriteValue(collection.StartIndex.Value);
            }

            if (collection.Members.Count > 0)
            {
                writer.WritePropertyName(Collection.MembersJName);
                writer.WriteStartArray();
                foreach (var member in collection.Members)
                    serializer.Serialize(writer, member);
                writer.WriteEndArray();
            }
        }
    }
}

