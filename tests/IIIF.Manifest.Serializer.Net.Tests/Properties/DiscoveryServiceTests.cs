using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using System.Linq;

namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class DiscoveryServiceTests
    {
        [Fact]
        public void DiscoveryService_SerializesCorrectly()
        {
            var discoveryService = new DiscoveryService(
                "http://iiif.io/api/discovery/1/context.json",
                "https://example.org/changes",
                "http://iiif.io/api/discovery/1/changes"
            );

            var @object = new ActivityObject("https://example.org/manifest/1", "Manifest");

            var activity = new Activity("Update", @object, "2023-01-01T00:00:00Z");

            discoveryService.AddActivity(activity);

            var json = JsonConvert.SerializeObject(discoveryService, Formatting.Indented);

            // Verify the JSON contains expected properties
            Assert.Contains("@context", json);
            Assert.Contains("@id", json);
            Assert.Contains("@type", json);
            Assert.Contains("profile", json);
            Assert.Contains("orderedItems", json);

            // Test round-trip deserialization
            var deserialized = JsonConvert.DeserializeObject<DiscoveryService>(json);
            Assert.NotNull(deserialized);
            Assert.Equal("https://example.org/changes", deserialized.Id);
            Assert.Equal("http://iiif.io/api/discovery/1/changes", deserialized.Profile);
            Assert.Single(deserialized.OrderedItems);
            Assert.Equal("Update", deserialized.OrderedItems.First().Type);
        }
    }
}