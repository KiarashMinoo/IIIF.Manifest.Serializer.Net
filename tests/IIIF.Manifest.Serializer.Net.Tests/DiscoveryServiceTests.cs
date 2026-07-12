using System.Linq;
using IIIF.Manifests.Serializer.Properties.Services;
using IIIF.Manifests.Serializer.Properties.Services.Discovery;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 12 (SDK_VERSIONING_GUIDE.md §10, finding 4): Change Discovery API 1.0 previously
///     conflated the top-level OrderedCollection with its pages (no first/last/totalItems/seeAlso/
///     partOf/rights, and "orderedItems" lived directly on the service), and Activity/ActivityObject
///     were missing several spec fields (id/startTime/summary/actor/target, canonical/seeAlso/provider).
/// </summary>
public class DiscoveryServiceTests
{
    [Fact]
    public void DiscoveryService_Should_HaveNoOrderedItems_AndPointAtPagesInstead()
    {
        var last = new DiscoveryResourceReference("https://example.org/activity/page-214", "OrderedCollectionPage");
        var service = new DiscoveryService("https://example.org/activity/all-changes", last)
            .SetFirst(new DiscoveryResourceReference("https://example.org/activity/page-0", "OrderedCollectionPage"))
            .SetTotalItems(21456)
            .SetRights("https://creativecommons.org/publicdomain/zero/1.0/")
            .AddSeeAlso(new DiscoveryDataset("https://example.org/dataset.jsonld").SetFormat("application/ld+json"))
            .AddPartOf(new DiscoveryResourceReference("https://example.org/activity/parent", "OrderedCollection"));

        var obj = JObject.Parse(service.Serialize());

        obj["orderedItems"].Should().BeNull();
        obj["type"]!.ToString().Should().Be("OrderedCollection");
        obj["first"]!["id"]!.ToString().Should().Be("https://example.org/activity/page-0");
        obj["last"]!["id"]!.ToString().Should().Be("https://example.org/activity/page-214");
        obj["totalItems"]!.Value<int>().Should().Be(21456);
        obj["rights"]!.ToString().Should().Be("https://creativecommons.org/publicdomain/zero/1.0/");
        obj["profile"].Should().BeNull();
    }

    [Fact]
    public void DiscoveryCollectionPage_Should_RoundTripPagingFieldsAndActivities()
    {
        var activity = new Activity("Update", new ActivityObject("https://example.org/manifest", "Manifest"), "2017-09-20T00:00:00Z");
        var page = new DiscoveryCollectionPage("https://example.org/activity/page-1", [activity])
            .SetContext(DiscoveryCollectionPage.DefaultContext)
            .SetPartOf(new DiscoveryResourceReference("https://example.org/activity/all-changes", "OrderedCollection"))
            .SetNext(new DiscoveryResourceReference("https://example.org/activity/page-2", "OrderedCollectionPage"))
            .SetPrev(new DiscoveryResourceReference("https://example.org/activity/page-0", "OrderedCollectionPage"))
            .SetStartIndex(100);

        var deserialized = TrackableObject.Parse<DiscoveryCollectionPage>(page.Serialize());

        deserialized.Type.Should().Be("OrderedCollectionPage");
        deserialized.PartOf!.Id.Should().Be("https://example.org/activity/all-changes");
        deserialized.Next!.Id.Should().Be("https://example.org/activity/page-2");
        deserialized.Prev!.Id.Should().Be("https://example.org/activity/page-0");
        deserialized.StartIndex.Should().Be(100);
        deserialized.OrderedItems.Single().Object.Id.Should().Be("https://example.org/manifest");
    }

    [Fact]
    public void Activity_MoveActivity_Should_RoundTripObjectAsSourceAndTargetAsDestination()
    {
        var activity = new Activity("Move",
                new ActivityObject("https://example.org/old-location/manifest", "Manifest"),
                "2017-09-21T00:00:00Z")
            .SetTarget(new DiscoveryResourceReference("https://example.org/new-location/manifest", "Manifest"));

        var deserialized = TrackableObject.Parse<Activity>(activity.Serialize());

        deserialized.Type.Should().Be("Move");
        deserialized.Object.Id.Should().Be("https://example.org/old-location/manifest");
        deserialized.Target!.Id.Should().Be("https://example.org/new-location/manifest");
    }

    [Fact]
    public void Activity_Should_RoundTripIdStartTimeSummaryAndActor()
    {
        var activity = new Activity("Create", new ActivityObject("https://example.org/manifest", "Manifest"), "2017-09-20T00:00:00Z")
            .SetId("https://example.org/activity/1")
            .SetStartTime("2017-09-19T23:00:00Z")
            .SetSummary("Initial publication")
            .SetActor(new DiscoveryAgent("https://example.org/about", "Organization").SetLabel("Example Organization"));

        var deserialized = TrackableObject.Parse<Activity>(activity.Serialize());

        deserialized.Id.Should().Be("https://example.org/activity/1");
        deserialized.StartTime.Should().Be("2017-09-19T23:00:00Z");
        deserialized.Summary.Should().Be("Initial publication");
        deserialized.Actor!.Type.Should().Be("Organization");
        deserialized.Actor!.Label.Single().Value.Should().Be("Example Organization");
    }

    [Fact]
    public void ActivityObject_Should_RoundTripCanonicalSeeAlsoAndProvider()
    {
        var activityObject = new ActivityObject("https://example.org/iiif/1/manifest", "Manifest")
            .SetCanonical("https://example.org/iiif/1")
            .AddSeeAlso(new DiscoveryDataset("https://example.org/dataset/single-item.jsonld")
                .SetLabel("Object Description").SetFormat("application/ld+json").SetProfile("https://schema.org/"))
            .AddProvider(new DiscoveryAgent("https://example.org/about", "Agent").SetLabel("Example Organization"));

        var deserialized = TrackableObject.Parse<ActivityObject>(activityObject.Serialize());

        deserialized.Canonical.Should().Be("https://example.org/iiif/1");
        deserialized.SeeAlso.Single().Format.Should().Be("application/ld+json");
        deserialized.Provider.Single().Label.Single().Value.Should().Be("Example Organization");
    }

    [Theory]
    [InlineData("Add")]
    [InlineData("Remove")]
    [InlineData("Delete")]
    public void Activity_Should_RoundTripAdditionalActivityTypes(string activityType)
    {
        // Activity.Type is a plain string (no closed enum), so every spec activity type - not just
        // Create/Update/Move, which already had dedicated tests - round-trips identically.
        var activity = new Activity(activityType, new ActivityObject("https://example.org/manifest", "Manifest"), "2017-09-20T00:00:00Z");

        var deserialized = TrackableObject.Parse<Activity>(activity.Serialize());

        deserialized.Type.Should().Be(activityType);
        deserialized.Object.Id.Should().Be("https://example.org/manifest");
    }

    [Fact]
    public void DiscoveryCollectionPage_Parse_Should_Throw_When_JsonIsMalformed()
    {
        var act = () => TrackableObject.Parse<DiscoveryCollectionPage>("{ this is not valid json");

        act.Should().Throw<JsonException>();
    }
}