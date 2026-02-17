using System;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Properties.DescriptionPropery;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples.Examples
{
    /// <summary>
    /// Example: Deserializing and modifying an existing IIIF manifest
    /// </summary>
    public static class DeserializeExample
    {
        public static void Run()
        {
            Console.WriteLine("=== Deserialize and Modify Example ===\n");

            // Sample IIIF manifest JSON
            string manifestJson = @"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://example.org/manifest/original"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Original Manifest"",
  ""metadata"": [
    {
      ""label"": ""Author"",
      ""value"": ""Unknown""
    }
  ],
  ""sequences"": [
    {
      ""@id"": ""https://example.org/sequence/s1"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://example.org/canvas/c1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""Page 1"",
          ""height"": 1000,
          ""width"": 800,
          ""images"": [
            {
              ""@id"": ""https://example.org/anno/a1"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://example.org/img1.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 1000,
                ""width"": 800
              },
              ""on"": ""https://example.org/canvas/c1""
            }
          ]
        }
      ]
    }
  ]
}";

            Console.WriteLine("Original JSON:");
            Console.WriteLine(manifestJson);
            Console.WriteLine("\n---\n");

            // Deserialize the manifest
            var manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson);

            Console.WriteLine($"Deserialized manifest ID: {manifest.Id}");
            Console.WriteLine($"Number of sequences: {manifest.Sequences.Count}");
            Console.WriteLine($"Metadata entries: {manifest.Metadata.Count}");

            // Modify the manifest
            manifest.SetMetadata("Editor", "Jane Doe")
                    .SetMetadata("Date", "2024")
                    .AddDescription(new Description("Modified and enhanced version."));

            // Serialize back to JSON
            var modifiedJson = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            Console.WriteLine("\n---\n");
            Console.WriteLine("Modified JSON:");
            Console.WriteLine(modifiedJson);

            Console.WriteLine("\n=== End of Deserialize Example ===\n");
        }
    }
}

