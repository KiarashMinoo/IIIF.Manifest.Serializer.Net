using System.Linq;
using IIIF.Manifests.Serializer.Examples;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties.Services.Search;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Issue #11 (SDK Phase 6B): the generic `ExampleCatalogTests` theory round-trips every demo
///     catalog entry through IiifSerializer/plain JsonConvert, but issue #11's own "Tests" section
///     asks for dedicated JSON-shape assertions per important demo pattern (search, image service,
///     annotation targeting, Range/TOC, collection/reference links, navPlace) - this file adds
///     those, plus builder-level assertions confirming each new/fixed demo actually exercises the
///     real SDK feature it claims to (not just a placeholder object).
/// </summary>
public class DemoCatalogTests
{
    private static T Find<T>(string title) where T : class
    {
        return (T)DemoCatalog.GetAll().Single(x => x.Title == title).Build();
    }

    [Fact]
    public void SearchDemo_Should_ExposeSearchAndAutoCompleteServiceDescriptors()
    {
        var manifest = Find<Manifest>("Wellcome-style search and access manifest");

        var json = IiifSerializer.Serialize(manifest);
        json.Should().Contain("SearchService2");
        json.Should().Contain("AutoCompleteService2");

        var obj = JObject.Parse(json);
        var serviceArray = obj["service"] as JArray;
        serviceArray.Should().NotBeNull();
        serviceArray!.OfType<JObject>().Should().Contain(x => x["type"]!.ToString() == "SearchService2");
    }

    [Fact]
    public void OverlaidSearchResults_Should_TargetCanvasRegionsWithTextQuoteHighlights()
    {
        var response = Find<SearchResponse>("Wellcome-style overlaid search results");

        response.Annotations.Should().NotBeNull();
        response.Annotations!.Items.Should().HaveCount(2);
        var firstHit = response.Annotations.Items.First();
        firstHit.Target.Source.Should().Contain("#xywh=");
        firstHit.Target.Selector.Should().ContainSingle(x => x.Exact == "lighthouse");
        response.PartOf!.Total.Should().Be(2);
    }

    [Fact]
    public void DeepZoomDemo_Should_ExposeImageServiceWithDimensions()
    {
        var manifest = Find<Manifest>("Nationalmuseum-style deep zoom manifest");
        var canvas = manifest.Items.OfType<Canvas>().Single();
        var image = (ImageResource)canvas.Items.OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body;

        image.Service.Should().ContainSingle();
        image.Height.Should().Be(4000);
        image.Width.Should().Be(3000);

        var json = IiifSerializer.Serialize(manifest);
        json.Should().Contain("ImageService2");
    }

    [Fact]
    public void StorytellingDemo_Should_TargetCanvasRegionsWithMultilingualCommentary()
    {
        var manifest = Find<Manifest>("Education/storytelling annotation tour");
        var canvas = manifest.Items.OfType<Canvas>().Single();
        var annotations = canvas.Items.OfType<AnnotationPage>().Single().Items.OfType<Annotation>().ToList();

        // One painting annotation (the photograph itself) plus 3 commentary annotations.
        annotations.Should().HaveCount(4);
        var commentary = annotations.Where(x => x.Motivation is "commenting" or "describing").ToList();
        commentary.Should().HaveCount(3);
        commentary.Should().Contain(x => x.Target.Selector != null);

        var mirrorAnnotations = commentary.Where(x => x.Id!.Contains("mirror")).ToList();
        mirrorAnnotations.Select(x => ((IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource.TextualBody)x.Body).Language).Should().BeEquivalentTo("en", "fr");
    }

    [Fact]
    public void GuidedTourDemo_Should_ExposeAnOrderedRangeOverASubsetOfCanvases()
    {
        var manifest = Find<Manifest>("Guided annotation tour (Range-based)");

        manifest.Items.OfType<Canvas>().Should().HaveCount(3);
        manifest.Structures.Should().ContainSingle();
        var tour = manifest.Structures.Single();
        tour.Canvases.Should().HaveCount(3);
        tour.Canvases.First().Should().Contain("canvas/1");
        tour.Canvases.Last().Should().Contain("canvas/3");

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["structures"]!.Should().ContainSingle();
        obj["structures"]![0]!["items"]!.Should().HaveCount(3);
    }

    [Fact]
    public void MixedMediaDemo_Should_ExposeAudioVideoImageCanvasesAndPdfAnd3DRenderings()
    {
        var manifest = Find<Manifest>("Mixed-media multi-canvas object (audio/video/PDF)");

        var canvases = manifest.Items.OfType<Canvas>().ToList();
        canvases.Should().HaveCount(3);

        var bodies = canvases.Select(c => c.Items.OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body).ToList();
        bodies.Should().ContainSingle(x => x is ImageResource);
        bodies.Should().ContainSingle(x => x is AudioResource);
        bodies.Should().ContainSingle(x => x is VideoResource);

        manifest.Rendering.Should().Contain(x => x.Format == "application/pdf");
        manifest.Rendering.Should().Contain(x => x.Format == "model/gltf-binary");
    }

    [Fact]
    public void ReunificationDemo_Should_PreserveEachSourceInstitutionsProviderAndHomepage()
    {
        var collection = Find<Collection>("Biblissima-style reunification of a separated object");

        var manifests = collection.Items.OfType<Manifest>().ToList();
        manifests.Should().HaveCount(2);

        foreach (var manifest in manifests)
        {
            manifest.Provider.Should().ContainSingle();
            manifest.Homepage.Should().NotBeEmpty();
            manifest.SeeAlso.Should().ContainSingle();
        }

        manifests.Select(x => x.Provider.Single().Label.Single().Value).Should()
            .BeEquivalentTo("Bibliothèque nationale de France", "Biblioteca Apostolica Vaticana");

        // Per spec, a Collection's "items" only ever reference nested Manifests by id/type/label
        // (WriteV3CollectionItem) - it never embeds their full content. The reunification pattern's
        // provenance data is preserved in the *in-memory* nested Manifest objects (asserted above)
        // and surfaces once a viewer follows the reference and fetches/renders each Manifest on
        // its own, which is what this checks.
        var collectionJson = IiifSerializer.Serialize(collection);
        var collectionObj = JObject.Parse(collectionJson);
        collectionObj["items"]!.Should().HaveCount(2);
        collectionObj["items"]!.OfType<JObject>().Select(x => x["provider"]).Should().OnlyContain(x => x == null);

        foreach (var manifest in manifests)
        {
            var manifestObj = JObject.Parse(IiifSerializer.Serialize(manifest));
            manifestObj["provider"].Should().NotBeNull();
        }
    }

    [Fact]
    public void MapDemo_Should_ExposeNavPlaceGeoJsonPoint()
    {
        var manifest = Find<Manifest>("Map-based place example");

        manifest.NavPlace.Should().NotBeNull();
        var feature = manifest.NavPlace!.Features.Single();
        feature.Geometry!.Type.Value.Should().Be("Point");

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["navPlace"]!["features"]![0]!["geometry"]!["type"]!.ToString().Should().Be("Point");
    }
}
