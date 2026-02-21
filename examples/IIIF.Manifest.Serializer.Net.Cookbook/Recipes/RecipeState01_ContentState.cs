using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// Recipe demonstrating IIIF Content State API 1.0 integration.
    /// Shows how to add content state services for deep linking and state representation.
    /// </summary>
    public static class RecipeState01_ContentState
    {
        public static string Recipe =
@"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/state-01/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Manifest with Content State"",
  ""service"": {
    ""@context"": ""http://iiif.io/api/content-state/1/context.json"",
    ""@id"": ""https://iiif.io/api/cookbook/recipe/state-01/content-state"",
    ""@type"": ""ContentStateService"",
    ""profile"": ""http://iiif.io/api/content-state/1/content-state""
  }
}";

        public static Manifest Create()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/state-01/manifest.json",
                new Label("Manifest with Content State")
            );

            // Add content state service
            var contentStateService = new ContentStateService(
                Context.ContentState1.Value,
                "https://iiif.io/api/cookbook/recipe/state-01/content-state",
                "http://iiif.io/api/content-state/1/content-state"
            );

            manifest.SetService(contentStateService);

            return manifest;
        }

        public static string ToJson()
        {
            var manifest = Create();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}