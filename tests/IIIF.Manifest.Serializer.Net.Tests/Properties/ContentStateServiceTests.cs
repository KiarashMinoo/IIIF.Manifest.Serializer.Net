using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class ContentStateServiceTests
    {
        [Fact]
        public void ContentStateService_SerializesCorrectly()
        {
            var contentStateService = new ContentStateService(
                "http://iiif.io/api/content-state/1/context.json",
                "https://example.org/content-state",
                "http://iiif.io/api/content-state/1/content-state"
            );

            var json = JsonConvert.SerializeObject(contentStateService, Formatting.Indented);

            // Verify the JSON contains expected properties
            Assert.Contains("@context", json);
            Assert.Contains("@id", json);
            Assert.Contains("@type", json);
            Assert.Contains("profile", json);

            // Test round-trip deserialization
            var deserialized = JsonConvert.DeserializeObject<ContentStateService>(json);
            Assert.NotNull(deserialized);
            Assert.Equal("https://example.org/content-state", deserialized.Id);
            Assert.Equal("http://iiif.io/api/content-state/1/content-state", deserialized.Profile);
        }
    }
}