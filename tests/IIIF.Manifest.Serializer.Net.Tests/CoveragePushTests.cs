using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 8 coverage push: targeted tests for legacy-only surfaces that the reshape
/// milestones didn't otherwise exercise (Collection paging, Structure viewing direction/start).
/// </summary>
public class CoveragePushTests
{
    [Fact]
    public void Collection_Paging_Should_RemainFunctional()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test"))
            .SetTotal(3)
            .SetFirst("https://example.org/collection?page=1")
            .SetLast("https://example.org/collection?page=3")
            .SetNext("https://example.org/collection?page=2")
            .SetPrev("https://example.org/collection?page=0")
            .SetStartIndex(0);

        collection.Total.Should().Be(3);
        collection.First.Should().Be("https://example.org/collection?page=1");
        collection.Last.Should().Be("https://example.org/collection?page=3");
        collection.Next.Should().Be("https://example.org/collection?page=2");
        collection.Prev.Should().Be("https://example.org/collection?page=0");
        collection.StartIndex.Should().Be(0);
    }

    [Fact]
    public void Structure_ViewingDirectionAndStartCanvas_Should_RemainFunctional()
    {
        var structure = new Structure("https://example.org/range/1", new Label("Chapter 1"))
            .SetViewingDirection(ViewingDirection.Rtl)
            .SetStartCanvas("https://example.org/canvas/1");

        structure.ViewingDirection!.Value.Should().Be("right-to-left");
        structure.StartCanvas.Should().Be("https://example.org/canvas/1");
    }
}
