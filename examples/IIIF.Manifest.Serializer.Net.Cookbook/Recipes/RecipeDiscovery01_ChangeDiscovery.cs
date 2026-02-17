using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples.Cookbook
{
    /// <summary>
    /// Recipe demonstrating IIIF Change Discovery API 1.0 integration.
    /// Shows how to add change discovery services to track manifest updates.
    /// </summary>
    public static class RecipeDiscovery01_ChangeDiscovery
    {
        public static string Recipe =
@"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/discovery-01/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Manifest with Change Discovery"",
  ""service"": {
    ""@context"": ""http://iiif.io/api/discovery/1/context.json"",
    ""@id"": ""https://iiif.io/api/cookbook/recipe/discovery-01/changes"",
    ""@type"": ""OrderedCollection"",
    ""profile"": ""http://iiif.io/api/discovery/1/changes"",
    ""orderedItems"": [
      {
        ""type"": ""Update"",
        ""object"": {
          ""id"": ""https://iiif.io/api/cookbook/recipe/discovery-01/manifest.json"",
          ""type"": ""Manifest""
        },
        ""endTime"": ""2023-01-01T00:00:00Z""
      }
    ]
  }
}";

        public static Manifest Create()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/discovery-01/manifest.json",
                new Label("Manifest with Change Discovery")
            );

            // Add change discovery service
            var discoveryService = new DiscoveryService(
                Context.Discovery1.Value,
                "https://iiif.io/api/cookbook/recipe/discovery-01/changes",
                "http://iiif.io/api/discovery/1/changes"
            );

            var activity = new Activity
            {
                Type = "Update",
                Object = new ActivityObject
                {
                    Id = "https://iiif.io/api/cookbook/recipe/discovery-01/manifest.json",
                    Type = "Manifest"
                },
                EndTime = "2023-01-01T00:00:00Z"
            };

            discoveryService.AddActivity(activity);
            manifest.SetService(discoveryService);

            return manifest;
        }

        public static string ToJson()
        {
            var manifest = Create();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}