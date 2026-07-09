using IIIF.Manifests.Serializer.Nodes;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

public abstract class CookbookExampleBase : ICookbookExample
{
    public abstract string Title { get; }

    public void Run()
    {
        var manifest = CreateManifest();
        var json = manifest.Serialize();

        Console.WriteLine(Title);
        Console.WriteLine("=".PadRight(60, '='));
        Console.WriteLine(json);
    }

    protected abstract Manifest CreateManifest();
}