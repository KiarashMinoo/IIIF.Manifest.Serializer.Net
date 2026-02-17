using IIIF.Manifests.Serializer.Nodes.CanvasNode;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Audio;
using IIIF.Manifests.Serializer.Nodes.ContentNode.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.ManifestNode.Manifest;

namespace IIIF.Manifests.Serializer.Net.Cookbook.Recipes
{
    /// <summary>
    /// Recipe 0002: Simplest Manifest - Audio
    /// https://iiif.io/api/cookbook/recipe/0002-mvm-audio/
    /// </summary>
    public static class Recipe002_Audio
    {
        public static string ToJson()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
                new Label("Simplest Audio Manifest")
            );

            var canvas = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/1",
                new Label("Audio Canvas"),
                640, 480
            ).SetDuration(1985.024);

            var audioResource = new AudioResource(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/audio/full/max/default.mp3",
                "audio/mp3"
            ).SetDuration(1985.024);

            var audio = new Audio(
                "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/annotation/audio",
                audioResource,
                canvas.Id
            );
            canvas.AddAudio(audio);

            var sequence = new Sequence("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/normal");
            sequence.AddCanvas(canvas);
            manifest.AddSequence(sequence);

            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}
