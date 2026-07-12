using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Properties;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

public sealed class Recipe0002SimplestAudioExample : CookbookExampleBase
{
    public override string Title => "Recipe 0002: Simplest Manifest - Single Audio File";

    protected override Manifest CreateManifest()
    {
        var audioResource = new AudioResource(
                "https://fixtures.iiif.io/audio/indiana/mahler-symphony-3/CD1/medium/128Kbps.mp4",
                "audio/mp4"
            )
            .SetDuration(1985.024);

        // In v2, Canvas requires height/width even for time-based media.
        var canvas = new Canvas(
            "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas",
            new Label("Mahler, Symphony No. 3: CD 1"),
            1,
            1
        );

        canvas.SetDuration(1985.024);

        var annotation = new Annotation(
            "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/canvas/page/annotation",
            audioResource,
            canvas.Id
        );

        canvas.AddAnnotation(annotation);

        var manifest = new Manifest(
            "https://iiif.io/api/cookbook/recipe/0002-mvm-audio/manifest.json",
            new Label("Simplest Audio Example 1")
        );
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://iiif.io/api/cookbook/recipe/0002-mvm-audio/sequence/s0");

        return manifest;
    }
}