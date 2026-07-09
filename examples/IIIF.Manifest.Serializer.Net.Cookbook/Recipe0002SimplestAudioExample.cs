using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Properties;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

public sealed class Recipe0002SimplestAudioExample : CookbookExampleBase
{
    public override string Title => "Recipe 0002: Simplest Manifest - Single Audio File";

    protected override Manifest CreateManifest()
    {
        var audioResource = new AudioResource(
                id: "https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4",
                format: "audio/mp4"
            )
            .SetDuration(1985.024);

        // In v2, Canvas requires height/width even for time-based media.
        var canvas = new Canvas(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas",
            label: new Label("Mahler, Symphony No. 3: CD 1"),
            height: 1,
            width: 1
        );

        canvas.SetDuration(1985.024);

        var audio = new Audio(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation",
            resource: audioResource,
            on: canvas.Id
        );

        canvas.AddAudio(audio);

        var sequence = new Sequence(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/s0"
        );
        sequence.AddCanvas(canvas);

        var manifest = new Manifest(
            id: "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
            label: new Label("Simplest Audio Example 1")
        );
        manifest.AddSequence(sequence);

        return manifest;
    }
}