using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
///     Shared construction helpers used across every <see cref="IRecipeSet" />: recipe-scoped id
///     builders, the Gottingen fixture ids reused by several recipes, and small factories -
///     NewManifest/NewCanvas/<see cref="PaintingImage" />/<see cref="WithSupplementaryPage" /> - for the
///     "single canvas + painting annotation" shape that most recipes start from.
/// </summary>
internal static class RecipeBuilders
{
    internal const string Base = "https://iiif.io/api/cookbook/recipe";
    internal const string ImageService3 = "http://iiif.io/api/image/3/context.json";

    internal const string GottingenImageId = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen/full/max/0/default.jpg";
    internal const string GottingenServiceId = "https://iiif.io/api/image/3.0/example/reference/918ecd18c2592080851777620de9bcb5-gottingen";

    internal static string Id(string recipe, string path)
    {
        return $"{Base}/{recipe}/{path}";
    }

    internal static string CanvasId(string recipe, string page)
    {
        return Id(recipe, $"canvas/{page}");
    }

    internal static Manifest NewManifest(string recipe, string label, string file = "manifest.json")
    {
        return new Manifest(Id(recipe, file), new Label(label));
    }

    internal static Manifest NewManifest(string recipe, Label label, string file = "manifest.json")
    {
        return new Manifest(Id(recipe, file), label);
    }

    internal static Manifest NewManifest(string recipe, Label primary, Label secondary)
    {
        return new Manifest(Id(recipe, "manifest.json"), primary).AddLabel(secondary);
    }

    internal static Canvas NewCanvas(string recipe, string page, string? label, int? height, int? width, string? idOverride = null, bool idIsFull = false, string labelLanguage = "en")
    {
        var id = idIsFull ? Id(recipe, page) : idOverride is not null ? Id(recipe, idOverride) : CanvasId(recipe, page);
        var canvas = new Canvas(id, new Label(label ?? "Untitled", label is null ? "none" : labelLanguage), height ?? 1, width ?? 1);
        return canvas;
    }

    internal static Canvas NewCanvas(string recipe, string page, string? label, int? height, int? width, string labelOverride, string labelLanguage)
    {
        return new Canvas(CanvasId(recipe, page), new Label(labelOverride, labelLanguage), height ?? 1, width ?? 1);
    }

    internal static Annotation PaintingImage(Canvas canvas, string recipe, string annotationPath, string imageId, string format, int? height, int? width, string? serviceId, bool idIsFull = false)
    {
        var image = new ImageResource(imageId, format);
        if (height is not null) image.SetHeight(height.Value);

        if (width is not null) image.SetWidth(width.Value);

        if (serviceId is not null) image.AddService(new Service(ImageService3, serviceId, "level1"));

        var annotationId = idIsFull ? Id(recipe, annotationPath) : Id(recipe, $"annotation/{annotationPath}");
        return new Annotation(annotationId, image, canvas.Id);
    }

    internal static Manifest WithSupplementaryPage(Manifest manifest, Canvas canvas, AnnotationPage page, Annotation annotation)
    {
        page.AddItem(annotation);
        return manifest.AddItem(canvas);
    }
}