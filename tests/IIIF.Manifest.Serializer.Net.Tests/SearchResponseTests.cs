using System.Linq;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource;
using IIIF.Manifests.Serializer.Properties.Services.Search;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 13 (SDK_VERSIONING_GUIDE.md §10, finding 5): Content Search API 2.0's actual response
/// shapes (the search AnnotationPage with hit highlighting/paging, and the autocomplete TermPage)
/// were entirely unmodeled - only the service *descriptors* (SearchService/AutoCompleteService)
/// existed.
/// </summary>
public class SearchResponseTests
{
    [Fact]
    public void SearchResponse_Should_WriteRequiredContextTypeAndItems()
    {
        var annotation = new Annotation("https://example.org/annotation/anno-bird",
            new EmbeddedContentResource("A bird in the hand is worth two in the bush", "en"),
            "https://example.org/canvas1#xywh=100,100,250,20");
        var response = new SearchResponse("https://example.org/service/manifest/search?q=bird").AddItem(annotation);

        var obj = JObject.Parse(response.Serialize());

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/search/2/context.json");
        obj["type"]!.ToString().Should().Be("AnnotationPage");
        obj["items"]!["target"]!.ToString().Should().Be("https://example.org/canvas1#xywh=100,100,250,20");
    }

    [Fact]
    public void SearchResponse_Should_RoundTripHitHighlightingAndPaging()
    {
        // Items (search-result Annotations) reuses the core Presentation 3.0 Annotation type;
        // its Body's polymorphic dispatch (Milestone 22, round 2) now round-trips through plain
        // JsonConvert/TrackableObject.Parse too, not just IiifSerializer's hand-built V3 reader.
        var annotation = new Annotation("https://example.org/annotation/anno-bird",
            new EmbeddedContentResource("A bird in the hand is worth two in the bush", "en"),
            "https://example.org/canvas1#xywh=100,100,250,20");

        var hitAnnotation = new SearchHitAnnotation(
                new SearchHitTarget("https://example.org/annotation/anno-bird",
                    [new SearchTextQuoteSelector("birds").SetPrefix("There are two ").SetSuffix(" in the bush")]))
            .SetId("https://example.org/annotation/match-1");

        var response = new SearchResponse("https://example.org/service/manifest/search?q=bird")
            .AddItem(annotation)
            .SetAnnotations(new SearchHitAnnotationPage([hitAnnotation]))
            .SetPartOf(new SearchAnnotationCollectionRef("https://example.org/search/results").SetTotal(1))
            .SetNext(new SearchResourceReference("https://example.org/search?page=2", "AnnotationPage"))
            .SetStartIndex(0)
            .AddIgnored("date");

        var deserialized = TrackableObject.Parse<SearchResponse>(response.Serialize());

        deserialized.Items.Single().Id.Should().Be("https://example.org/annotation/anno-bird");
        var hit = deserialized.Annotations!.Items.Single();
        hit.Motivation.Should().Be("contextualizing");
        hit.Target.Source.Should().Be("https://example.org/annotation/anno-bird");
        hit.Target.Selector.Single().Exact.Should().Be("birds");
        hit.Target.Selector.Single().Prefix.Should().Be("There are two ");
        deserialized.PartOf!.Total.Should().Be(1);
        deserialized.Next!.Id.Should().Be("https://example.org/search?page=2");
        deserialized.StartIndex.Should().Be(0);
        deserialized.Ignored.Should().ContainSingle("date");
    }

    [Fact]
    public void TermPageResponse_Should_RoundTripMinimalAndExtendedTerms()
    {
        var response = new TermPageResponse("https://example.org/service/identifier/autocomplete?q=bir")
            .AddItem(new SearchTerm("bird"))
            .AddItem(new SearchTerm("https://semtag.example.org/tag/biro").SetType("Term").SetTotal(3).SetLabel("biro"))
            .AddIgnored("user");

        var obj = JObject.Parse(response.Serialize());
        obj["type"]!.ToString().Should().Be("TermPage");

        var deserialized = TrackableObject.Parse<TermPageResponse>(response.Serialize());

        deserialized.Items.Should().HaveCount(2);
        deserialized.Items.First().Value.Should().Be("bird");
        deserialized.Items.First().Type.Should().BeNull();
        var extended = deserialized.Items.Last();
        extended.Total.Should().Be(3);
        extended.Label.Single().Value.Should().Be("biro");
        deserialized.Ignored.Should().ContainSingle("user");
    }
}
