using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Video;
using IIIF.Manifests.Serializer.Nodes.Content.Video.Resource;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest.Manifest;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0003: Simplest Manifest - Video
    /// https://iiif.io/api/cookbook/recipe/0003-mvm-video/
    /// </summary>
    public static class Recipe003_Video
    {
        public static string ToJson()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/manifest.json",
                new Label("Simplest Video Manifest")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/canvas/1",
                new Label("Video Canvas"),
                640, 480
            ).SetDuration(660.0);

            var videoResource = new VideoResource(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/video/full/max/default.mp4",
                "video/mp4"
            ).SetDuration(660.0);

            var video = new Video(
                "https://iiif.io/api/cookbook/recipe/0003-mvm-video/annotation/video",
                videoResource,
                canvas.Id
            );
            canvas.AddVideo(video);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0003-mvm-video/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}