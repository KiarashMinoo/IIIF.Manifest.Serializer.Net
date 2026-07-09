using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IIIF.Manifests.Serializer.Examples;
using IIIF.Manifests.Serializer.Net.Cookbook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

public class ExampleCatalogTests
{
    public static IEnumerable<object[]> CookbookExamples => CookbookCatalog.GetAll().Select(example => new object[] { example });
    public static IEnumerable<object[]> DemoExamples => DemoCatalog.GetAll().Select(example => new object[] { example });

    [Theory]
    [MemberData(nameof(CookbookExamples))]
    public void Cookbook_examples_should_serialize(IIIF.Manifests.Serializer.Net.Cookbook.ExampleDefinition example)
    {
        AssertSerializes(example.Title, example.Build());
    }

    [Theory]
    [MemberData(nameof(DemoExamples))]
    public void Demo_examples_should_serialize(IIIF.Manifests.Serializer.Examples.ExampleDefinition example)
    {
        AssertSerializes(example.Title, example.Build());
    }

    private static void AssertSerializes(string title, object value)
    {
        title.Should().NotBeNullOrWhiteSpace();

        var json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        json.Should().NotBeNullOrWhiteSpace();
        JToken.Parse(json).Should().NotBeNull();
    }
}




