using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

public sealed class Recipe0001SimplestImageExample : CookbookExampleBase
{
    public override string Title => "Recipe 0001: Simplest Manifest - Single Image File";

    protected override Manifest CreateManifest()
    {
        var imageResource = new ImageResource(
                "http://iiif.io/api/presentation/2.1/example/fixtures/resources/page1-full.png",
                "image/png"
            )
            .SetHeight(1800)
            .SetWidth(1200);

        var canvas = new Canvas(
            "https://iiif.io/api/cookbook/recipe/0001-mvm-image/canvas/p1",
            new Label("p. 1"),
            1800,
            1200
        );

        var annotation = new Annotation(
            "https://iiif.io/api/cookbook/recipe/0001-mvm-image/annotation/p0001-image",
            imageResource,
            canvas.Id
        );

        canvas.AddAnnotation(annotation);

        var manifest = new Manifest(
            "https://iiif.io/api/cookbook/recipe/0001-mvm-image/manifest.json",
            new Label("Single Image Example")
        );
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://iiif.io/api/cookbook/recipe/0001-mvm-image/sequence/s0");

        manifest.AddContext("TestContext");

        return manifest;
    }
}