using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 3 (SDK_VERSIONING_GUIDE.md): Collection reshaped around 3.0-native Items
/// (heterogeneous Manifest/Collection references), with Collections/Manifests/Members as
/// computed legacy views. IiifSerializer gains full Collection read/write support.
/// </summary>
public class CollectionReshapeTests
{
    [Fact]
    public void AddItem_Should_PopulateItemsAsThePrimary3_0Storage()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"));
        var manifest = new Manifest("https://example.org/manifest", new Label("M"));
        var nested = new Collection("https://example.org/collection/sub", new Label("Sub"));

        collection.AddItem(manifest);
        collection.AddItem(nested);

        collection.Items.Should().HaveCount(2);
    }

    [Fact]
    public void ManifestsAndCollections_Should_BeComputedLegacyViews_ReflectingItems()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"));
        collection.AddManifestReference("https://example.org/manifest/1");
        collection.AddItem(new Collection("https://example.org/collection/sub", new Label("Sub")));

        collection.Manifests.Should().ContainSingle(x => x == "https://example.org/manifest/1");
        collection.Collections.Should().ContainSingle(x => x.Id == "https://example.org/collection/sub");
        collection.Members.Should().HaveCount(2);
    }

    [Fact]
    public void LegacyCollectionJson_Should_DeserializeDirectlyIntoItems()
    {
        const string json = """
                            {
                              "@id": "https://example.org/collection",
                              "@type": "sc:Collection",
                              "label": "Test",
                              "manifests": ["https://example.org/manifest/1", "https://example.org/manifest/2"],
                              "collections": [
                                { "@id": "https://example.org/collection/sub", "@type": "sc:Collection", "label": "Sub" }
                              ]
                            }
                            """;

        var collection = JsonConvert.DeserializeObject<Collection>(json)!;

        collection.Items.Should().HaveCount(3);
        collection.Manifests.Should().BeEquivalentTo(["https://example.org/manifest/1", "https://example.org/manifest/2"]);
        collection.Collections.Should().ContainSingle(x => x.Id == "https://example.org/collection/sub");
    }

    [Fact]
    public void Collection_Should_NotLeak3_0ItemsIntoLegacyV2Json()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"));
        collection.AddManifestReference("https://example.org/manifest/1");

        var json = JsonConvert.SerializeObject(collection);
        var obj = JObject.Parse(json);

        obj["manifests"].Should().NotBeNull();
        obj["items"].Should().BeNull("Items is 3.0-native storage and must never leak into legacy JSON via reflection-based serialization");
    }

    [Fact]
    public void IiifSerializer_Should_WriteV3Collection()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"));
        collection.AddManifestReference("https://example.org/manifest/1");
        collection.AddItem(new Collection("https://example.org/collection/sub", new Label("Sub")));

        var json = IiifSerializer.Serialize(collection);
        var obj = JObject.Parse(json);

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/3/context.json");
        obj["type"]!.ToString().Should().Be("Collection");
        obj["items"]!.Should().HaveCount(2);
        obj["items"]!.OfType<JObject>().Should().Contain(x => x["type"]!.ToString() == "Manifest" && x["id"]!.ToString() == "https://example.org/manifest/1");
        obj["items"]!.OfType<JObject>().Should().Contain(x => x["type"]!.ToString() == "Collection" && x["id"]!.ToString() == "https://example.org/collection/sub");
        obj["manifests"].Should().BeNull();
    }

    [Fact]
    public void IiifSerializer_Should_WriteLegacyV2Collection()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"));
        collection.AddManifestReference("https://example.org/manifest/1");

        var json = IiifSerializer.Serialize(collection, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        var obj = JObject.Parse(json);

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/presentation/2/context.json");
        obj["@type"]!.ToString().Should().Be("sc:Collection");
        var manifests = obj["manifests"] as JArray;
        manifests.Should().NotBeNull();
        manifests!.Values<string>().Should().ContainSingle(x => x == "https://example.org/manifest/1");
        obj["items"].Should().BeNull();
    }

    [Fact]
    public void IiifSerializer_Should_WriteTheSameLegacyShape_When_CollectionVersionIsV2_0()
    {
        // 2.0 and 2.1 are wire-format-identical for Collection too (mirrors the equivalent
        // Manifest test in IiifSerializerTests.cs) - explicitly requesting V2_0 must succeed and
        // match V2_1's output, not throw.
        var collection = new Collection("https://example.org/collection", new Label("Test"));
        collection.AddManifestReference("https://example.org/manifest/1");

        var v2_0Json = IiifSerializer.Serialize(collection, new IiifSerializerOptions(IiifPresentationVersion.V2_0));
        var v2_1Json = IiifSerializer.Serialize(collection, new IiifSerializerOptions(IiifPresentationVersion.V2_1));

        v2_0Json.Should().Be(v2_1Json);
    }

    [Fact]
    public void IiifSerializer_Should_ThrowNotSupported_When_VersionIsV4_0Rc()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"));

        var act = () => IiifSerializer.Serialize(collection, new IiifSerializerOptions(IiifPresentationVersion.V4_0_Rc));

        act.Should().Throw<NotSupportedException>().WithMessage("*V4_0_Rc*");
    }

    [Fact]
    public void DeserializeCollection_Should_ThrowNotSupported_When_VersionIsDetectedButUnimportable()
    {
        const string json = """
                            {
                              "@context": "http://www.shared-canvas.org/ns/context.json",
                              "@id": "https://example.org/collection",
                              "@type": "sc:Collection",
                              "manifests": []
                            }
                            """;

        var act = () => IiifSerializer.DeserializeCollection(json);

        act.Should().Throw<NotSupportedException>().WithMessage("*Metadata_1_0*");
    }

    [Fact]
    public void IiifSerializer_Should_ReadV3Collection()
    {
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/collection",
                              "type": "Collection",
                              "label": { "none": ["Test"] },
                              "items": [
                                { "id": "https://example.org/manifest/1", "type": "Manifest", "label": { "none": ["M1"] } },
                                { "id": "https://example.org/collection/sub", "type": "Collection", "label": { "none": ["Sub"] } }
                              ]
                            }
                            """;

        var collection = IiifSerializer.DeserializeCollection(json);

        collection.Manifests.Should().ContainSingle(x => x == "https://example.org/manifest/1");
        collection.Collections.Should().ContainSingle(x => x.Id == "https://example.org/collection/sub");
    }

    [Fact]
    public void LegacyCollection_Should_CrossSerializeAsV3()
    {
        const string json = """
                            {
                              "@id": "https://example.org/collection",
                              "@type": "sc:Collection",
                              "label": "Test",
                              "manifests": ["https://example.org/manifest/1"]
                            }
                            """;

        var collection = IiifSerializer.DeserializeCollection(json);
        var v3Json = IiifSerializer.Serialize(collection);
        var obj = JObject.Parse(v3Json);

        obj["items"]!.Should().ContainSingle();
        obj["items"]![0]!["id"]!.ToString().Should().Be("https://example.org/manifest/1");
        obj["items"]![0]!["type"]!.ToString().Should().Be("Manifest");
    }

    [Fact]
    public void PagingProperties_Should_RemainFunctional_AndNotBeObsolete()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"))
            .SetTotal(2)
            .SetFirst("https://example.org/collection?page=1")
            .SetLast("https://example.org/collection?page=2");

        collection.Total.Should().Be(2);
        collection.First.Should().Be("https://example.org/collection?page=1");
        collection.Last.Should().Be("https://example.org/collection?page=2");

        typeof(Collection).GetMethod(nameof(Collection.SetTotal))!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }

    [Theory]
    [InlineData(nameof(Collection.AddCollection))]
    [InlineData(nameof(Collection.RemoveCollection))]
    [InlineData(nameof(Collection.AddManifest))]
    [InlineData(nameof(Collection.RemoveManifest))]
    [InlineData(nameof(Collection.AddMember))]
    [InlineData(nameof(Collection.RemoveMember))]
    public void LegacyMutators_Should_BeMarkedObsoleteAsWarnings(string methodName)
    {
        var method = typeof(Collection).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var obsolete = method!.GetCustomAttribute<System.ObsoleteAttribute>();
        obsolete.Should().NotBeNull();
        obsolete!.IsError.Should().BeFalse("legacy mutators remain callable - deprecated with a warning, not a compile-time error");
    }

    [Theory]
    [InlineData(nameof(Collection.AddCollection))]
    [InlineData(nameof(Collection.RemoveCollection))]
    [InlineData(nameof(Collection.AddManifest))]
    [InlineData(nameof(Collection.RemoveManifest))]
    [InlineData(nameof(Collection.AddMember))]
    [InlineData(nameof(Collection.RemoveMember))]
    public void LegacyMutators_Should_CarryIIIFVersionAttribute_DescribingTheDeprecation(string methodName)
    {
        var method = typeof(Collection).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        var version = method!.GetCustomAttribute<IIIFVersionAttribute>();
        version.Should().NotBeNull("legacy mutators must document their deprecation via an IIIFVersionAttribute-derived attribute");
        version!.IsDeprecated.Should().BeTrue();
        version.DeprecatedInVersion.Should().Be("3.0");
        version.ReplacedBy.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData(nameof(Collection.Collections))]
    [InlineData(nameof(Collection.Manifests))]
    [InlineData(nameof(Collection.Members))]
    public void LegacyGetters_Should_NotBeObsolete(string propertyName)
    {
        var property = typeof(Collection).GetProperty(propertyName);

        property.Should().NotBeNull();
        property!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }

    [Fact]
    public void AddManifestReference_Should_NotBeObsolete()
    {
        var method = typeof(Collection).GetMethod(nameof(Collection.AddManifestReference), BindingFlags.Instance | BindingFlags.Public);

        method.Should().NotBeNull();
        method!.GetCustomAttribute<System.ObsoleteAttribute>().Should().BeNull();
    }
}
