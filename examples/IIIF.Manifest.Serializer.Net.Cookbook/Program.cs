using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

internal static class Program
{
    private static void Main(string[] args)
    {
        foreach (var example in CookbookCatalog.GetAll())
        {
            example.Run();
            Console.WriteLine();
        }
    }
}
