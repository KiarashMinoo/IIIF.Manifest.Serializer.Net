using IIIF.Manifests.Serializer.Nodes.Canvas;
using IIIF.Manifests.Serializer.Nodes.Content.Image;
using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Sequence;
using IIIFManifest = IIIF.Manifests.Serializer.Nodes.Manifest.Manifest;
using IIIF.Manifests.Serializer.Nodes.Structure;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifest.Serializer.Cookbook.Recipes
{
    /// <summary>
    /// IIIF Cookbook Recipe 0009: Simple Manifest — Book
    /// https://iiif.io/api/cookbook/recipe/0009-book-1/
    ///
    /// A multi-page book manifest with viewingHint=paged and
    /// hierarchical structures (ranges) for table of contents.
    /// v3 behavior ["paged"] → v2 viewingHint "paged".
    /// </summary>
    public static class Recipe003_BookWithTOC
    {
        public static IIIFManifest Create()
        {
            var manifest = new IIIFManifest(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/manifest.json",
                new Label("Simple Manifest - Book")
            );

            manifest.SetViewingDirection(ViewingDirection.Ltr);
            manifest.SetViewingHint(ViewingHint.Paged);

            var sequence = new Sequence(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/sequence/normal"
            );

            // Page 1: Blank page (front cover)
            var canvas1 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1",
                new Label("Blank page"),
                1800, 1200
            );
            var img1 = new Image(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0001-image",
                new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-24/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200),
                canvas1.Id
            );
            canvas1.AddImage(img1);
            sequence.AddCanvas(canvas1);

            // Page 2: Frontispiece
            var canvas2 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p2",
                new Label("Frontispiece"),
                1800, 1200
            );
            var img2 = new Image(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0002-image",
                new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-25/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200),
                canvas2.Id
            );
            canvas2.AddImage(img2);
            sequence.AddCanvas(canvas2);

            // Page 3: Title page
            var canvas3 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p3",
                new Label("Title page"),
                1800, 1200
            );
            var img3 = new Image(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0003-image",
                new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-26/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200),
                canvas3.Id
            );
            canvas3.AddImage(img3);
            sequence.AddCanvas(canvas3);

            // Page 4
            var canvas4 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p4",
                new Label("Page 4"),
                1800, 1200
            );
            var img4 = new Image(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0004-image",
                new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-27/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200),
                canvas4.Id
            );
            canvas4.AddImage(img4);
            sequence.AddCanvas(canvas4);

            // Page 5
            var canvas5 = new Canvas(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p5",
                new Label("Page 5"),
                1800, 1200
            );
            var img5 = new Image(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/annotation/p0005-image",
                new ImageResource(
                    "https://iiif.io/api/image/3.0/example/reference/59d09e6773341f28ea166e592f764e4f-28/full/max/0/default.jpg",
                    "image/jpeg"
                ).SetHeight(1800).SetWidth(1200),
                canvas5.Id
            );
            canvas5.AddImage(img5);
            sequence.AddCanvas(canvas5);

            manifest.AddSequence(sequence);

            // Table of Contents via structures (ranges)
            var toc = new Structure(
                "https://iiif.io/api/cookbook/recipe/0009-book-1/range/r0",
                new Label("Table of Contents")
            );
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p1");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p2");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p3");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p4");
            toc.AddCanvas("https://iiif.io/api/cookbook/recipe/0009-book-1/canvas/p5");

            manifest.AddStructure(toc);

            return manifest;
        }

        public static string ToJson()
        {
            var manifest = Create();
            return JsonConvert.SerializeObject(manifest, Formatting.Indented);
        }
    }
}

