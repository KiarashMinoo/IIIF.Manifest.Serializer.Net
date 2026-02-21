using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.ManifestNode;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// Recipe demonstrating IIIF Content Search API 2.0 integration.
    /// Shows how to add search services to a manifest for full-text search capabilities.
    /// </summary>
    public static class RecipeSearch01_ContentSearch
    {
        public static string Recipe =
@"{
  ""@context"": ""http://iiif.io/api/presentation/2/context.json"",
  ""@id"": ""https://iiif.io/api/cookbook/recipe/search-01/manifest.json"",
  ""@type"": ""sc:Manifest"",
  ""label"": ""Search within a Manuscript"",
  ""service"": {
    ""@context"": ""http://iiif.io/api/search/2/context.json"",
    ""@id"": ""https://iiif.io/api/cookbook/recipe/search-01/search"",
    ""@type"": ""SearchService2"",
    ""profile"": ""http://iiif.io/api/search/2/search"",
    ""service"": {
      ""@context"": ""http://iiif.io/api/search/2/context.json"",
      ""@id"": ""https://iiif.io/api/cookbook/recipe/search-01/autocomplete"",
      ""@type"": ""AutoCompleteService2"",
      ""profile"": ""http://iiif.io/api/search/2/autocomplete""
    }
  },
  ""sequences"": [
    {
      ""@id"": ""https://iiif.io/api/cookbook/recipe/search-01/sequence/normal"",
      ""@type"": ""sc:Sequence"",
      ""canvases"": [
        {
          ""@id"": ""https://iiif.io/api/cookbook/recipe/search-01/canvas/p1"",
          ""@type"": ""sc:Canvas"",
          ""label"": ""p. 1"",
          ""height"": 3000,
          ""width"": 2000,
          ""images"": [
            {
              ""@id"": ""https://iiif.io/api/cookbook/recipe/search-01/annotation/p0001-image"",
              ""@type"": ""oa:Annotation"",
              ""motivation"": ""sc:painting"",
              ""resource"": {
                ""@id"": ""https://iiif.io/api/image/2/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg"",
                ""@type"": ""dctypes:Image"",
                ""format"": ""image/jpeg"",
                ""height"": 3000,
                ""width"": 2000,
                ""service"": {
                  ""@context"": ""http://iiif.io/api/image/2/context.json"",
                  ""@id"": ""https://iiif.io/api/image/2/example/reference/918ecd18c2592080851777620de9bcb5-gottingen"",
                  ""@type"": ""ImageService2"",
                  ""profile"": ""http://iiif.io/api/image/2/level1.json""
                }
              },
              ""on"": ""https://iiif.io/api/cookbook/recipe/search-01/canvas/p1""
            }
          ]
        }
      ]
    }
  ]
}";

        public static Manifest Create()
        {
            var manifest = new Manifest(
                "https://iiif.io/api/cookbook/recipe/search-01/manifest.json",
                new Label("Search within a Manuscript")
            );

            // Add search service with autocomplete
            var searchService = new SearchService(
                Context.Search2.Value,
                "https://iiif.io/api/cookbook/recipe/search-01/search",
                "http://iiif.io/api/search/2/search"
            );

            var autoCompleteService = new AutoCompleteService(
                Context.Search2.Value,
                "https://iiif.io/api/cookbook/recipe/search-01/autocomplete",
                "http://iiif.io/api/search/2/autocomplete"
            );

            searchService.AddService(autoCompleteService);
            manifest.SetService(searchService);

            // Add sequence and canvas as usual
            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/search-01/sequence/normal");
            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/search-01/canvas/p1",
                new Label("p. 1"),
                3000,
                2000
            );

            // Add image with IIIF Image Service
            var imageResource = new ImageResource(
                "https://iiif.io/api/image/2/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg",
                "image/jpeg"
            ).SetHeight(3000).SetWidth(2000);

            var imageService = new Service(
                Context.Image2.Value,
                "https://iiif.io/api/image/2/example/reference/918ecd18c2592080851777620de9bcb5-gottingen",
                "http://iiif.io/api/image/2/level1.json"
            );

            imageResource.SetService(imageService);

            var image = new Image(
                "https://iiif.io/api/cookbook/recipe/search-01/annotation/p0001-image",
                imageResource,
                canvas.Id
            );

            canvas.AddImage(image);
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            return manifest;
        }

        public static string ToJson()
        {
            var manifest = Create();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}