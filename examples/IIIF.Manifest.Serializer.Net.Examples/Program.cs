using System;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Examples;

internal static class Program
{
    private static void Main(string[] args)
    {
        foreach (var example in DemoCatalog.GetAll())
        {
            Console.WriteLine(example.Title);
            Console.WriteLine(new string('=', example.Title.Length));
            Console.WriteLine(JsonConvert.SerializeObject(example.Build(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
            Console.WriteLine();
        }
    }
}