using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Helpers
{
    /// <summary>
    /// Helper class for reading and writing JSON collection properties that can be either single values or arrays.
    /// This eliminates repetitive converter code for handling collection properties.
    /// </summary>
    public static class JsonCollectionPropertyHelper
    {
        /// <summary>
        /// Reads a property from JSON that can be either a single value or an array, and populates the target object
        /// using the provided add method.
        /// </summary>
        /// <typeparam name="TNode">The type of the node being populated</typeparam>
        /// <typeparam name="TProperty">The type of the property value</typeparam>
        /// <param name="element">The JToken containing the JSON</param>
        /// <param name="node">The node to populate</param>
        /// <param name="propertyName">The JSON property name</param>
        /// <param name="addMethod">Method to add a single item to the node</param>
        /// <returns>The node with properties populated</returns>
        public static TNode ReadCollectionProperty<TNode, TProperty>(
            JToken element,
            TNode node,
            string propertyName,
            Action<TNode, TProperty> addMethod)
        {
            var jToken = element.TryGetToken(propertyName);
            if (jToken != null)
            {
                if (jToken is JArray)
                {
                    var items = jToken.ToObject<TProperty[]>();
                    if (items != null)
                    {
                        foreach (var item in items)
                            addMethod(node, item);
                    }
                }
                else
                {
                    var item = jToken.ToObject<TProperty>();
                    if (item != null)
                        addMethod(node, item);
                }
            }

            return node;
        }

        /// <summary>
        /// Writes a collection property to JSON. If count is 1, writes a single value; otherwise writes an array.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property value</typeparam>
        /// <param name="writer">The JsonWriter</param>
        /// <param name="serializer">The JsonSerializer</param>
        /// <param name="propertyName">The JSON property name</param>
        /// <param name="collection">The collection to write</param>
        /// <param name="writeSingleAsArray">If true, always write as array even when count is 1</param>
        public static void WriteCollectionProperty<TProperty>(
            JsonWriter writer,
            JsonSerializer serializer,
            string propertyName,
            IReadOnlyCollection<TProperty> collection,
            bool writeSingleAsArray = false)
        {
            if (!collection.Any())
                return;

            writer.WritePropertyName(propertyName);

            if (collection.Count == 1 && !writeSingleAsArray)
            {
                serializer.Serialize(writer, collection.First());
            }
            else
            {
                writer.WriteStartArray();
                foreach (var item in collection)
                    serializer.Serialize(writer, item);
                writer.WriteEndArray();
            }
        }

        /// <summary>
        /// Writes a collection property to JSON, always as an array regardless of count.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property value</typeparam>
        /// <param name="writer">The JsonWriter</param>
        /// <param name="serializer">The JsonSerializer</param>
        /// <param name="propertyName">The JSON property name</param>
        /// <param name="collection">The collection to write</param>
        public static void WriteCollectionPropertyAsArray<TProperty>(
            JsonWriter writer,
            JsonSerializer serializer,
            string propertyName,
            IReadOnlyCollection<TProperty> collection)
        {
            WriteCollectionProperty(writer, serializer, propertyName, collection, writeSingleAsArray: true);
        }
    }
}

