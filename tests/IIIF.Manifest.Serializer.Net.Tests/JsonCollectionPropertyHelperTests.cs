using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     <see cref="JsonCollectionPropertyHelper" /> is the shared single-value-or-array read/write helper
///     used to eliminate repetitive converter code - it had zero direct test coverage even though every
///     hand-rolled converter in the codebase relies on the same "single value collapses, multiple
///     values become an array" behavior it implements generically here.
/// </summary>
public class JsonCollectionPropertyHelperTests
{
    [Fact]
    public void ReadCollectionProperty_Should_AddASingleItem_WhenTokenIsABareValue()
    {
        var element = JObject.Parse("""{"label": "one"}""");
        var items = new List<string>();

        JsonCollectionPropertyHelper.ReadCollectionProperty<List<string>, string>(
            element, items, "label", (list, value) => list.Add(value));

        items.Should().ContainSingle().Which.Should().Be("one");
    }

    [Fact]
    public void ReadCollectionProperty_Should_AddEveryItem_WhenTokenIsAnArray()
    {
        var element = JObject.Parse("""{"label": ["one", "two", "three"]}""");
        var items = new List<string>();

        JsonCollectionPropertyHelper.ReadCollectionProperty<List<string>, string>(
            element, items, "label", (list, value) => list.Add(value));

        items.Should().Equal("one", "two", "three");
    }

    [Fact]
    public void ReadCollectionProperty_Should_AddNothing_WhenPropertyIsMissing()
    {
        var element = JObject.Parse("{}");
        var items = new List<string>();

        JsonCollectionPropertyHelper.ReadCollectionProperty<List<string>, string>(
            element, items, "label", (list, value) => list.Add(value));

        items.Should().BeEmpty();
    }

    [Fact]
    public void WriteCollectionProperty_Should_WriteABareValue_WhenCollectionHasExactlyOneItem()
    {
        using var writer = new JTokenWriter();
        var serializer = new JsonSerializer();
        writer.WriteStartObject();

        JsonCollectionPropertyHelper.WriteCollectionProperty(writer, serializer, "label", new[] { "solo" });

        writer.WriteEndObject();
        var obj = (JObject)writer.Token!;

        obj["label"]!.Type.Should().Be(JTokenType.String);
        obj["label"]!.ToString().Should().Be("solo");
    }

    [Fact]
    public void WriteCollectionProperty_Should_WriteAnArray_WhenCollectionHasMultipleItems()
    {
        using var writer = new JTokenWriter();
        var serializer = new JsonSerializer();
        writer.WriteStartObject();

        JsonCollectionPropertyHelper.WriteCollectionProperty(writer, serializer, "label", new[] { "one", "two" });

        writer.WriteEndObject();
        var obj = (JObject)writer.Token!;

        obj["label"].Should().BeOfType<JArray>();
        obj["label"]!.Values<string>().Should().Equal("one", "two");
    }

    [Fact]
    public void WriteCollectionProperty_Should_WriteNothing_WhenCollectionIsEmpty()
    {
        using var writer = new JTokenWriter();
        var serializer = new JsonSerializer();
        writer.WriteStartObject();

        JsonCollectionPropertyHelper.WriteCollectionProperty(writer, serializer, "label", Array.Empty<string>());

        writer.WriteEndObject();
        var obj = (JObject)writer.Token!;

        obj["label"].Should().BeNull();
    }

    [Fact]
    public void WriteCollectionPropertyAsArray_Should_WriteAnArray_EvenWithExactlyOneItem()
    {
        using var writer = new JTokenWriter();
        var serializer = new JsonSerializer();
        writer.WriteStartObject();

        JsonCollectionPropertyHelper.WriteCollectionPropertyAsArray(writer, serializer, "label", new[] { "solo" });

        writer.WriteEndObject();
        var obj = (JObject)writer.Token!;

        obj["label"].Should().BeOfType<JArray>();
        obj["label"]!.Values<string>().Should().ContainSingle().Which.Should().Be("solo");
    }
}