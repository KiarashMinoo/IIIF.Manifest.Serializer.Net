using System;
using System.IO;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using Newtonsoft.Json;

namespace TestUnhandledProperties
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing unhandled properties storage...");
            
            // Read test manifest with custom properties
            var json = File.ReadAllText("test_unhandled_properties.json");
            
            // Deserialize
            var manifest = JsonConvert.DeserializeObject<Manifest>(json);
            
            Console.WriteLine($"Manifest ID: {manifest.Id}");
            Console.WriteLine($"Manifest Label: {manifest.Label.FirstOrDefault()?.Value}");
            
            // Check if additional properties were stored
            Console.WriteLine($"\nAdditional properties count: {manifest.AdditionalProperties.Count}");
            
            foreach (var prop in manifest.AdditionalProperties)
            {
                Console.WriteLine($"  {prop.Key}: {prop.Value}");
            }
            
            // Re-serialize and verify round-trip
            var reserialized = JsonConvert.SerializeObject(manifest, Formatting.Indented);
            
            Console.WriteLine("\nRe-serialized manifest:");
            Console.WriteLine(reserialized);
            
            // Verify custom properties are preserved
            if (reserialized.Contains("customProperty"))
            {
                Console.WriteLine("\n✓ Custom properties preserved!");
            }
            else
            {
                Console.WriteLine("\n✗ Custom properties NOT preserved!");
            }
        }
    }
}

