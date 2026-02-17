using IIIF.Manifests.Serializer.Properties.ServiceProperty;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class SearchServiceTests
    {
        [Fact]
        public void SearchService_SerializesCorrectly()
        {
            var searchService = new SearchService(
                "http://iiif.io/api/search/2/context.json",
                "https://example.org/search",
                "http://iiif.io/api/search/2/search"
            );

            var autoCompleteService = new AutoCompleteService(
                "http://iiif.io/api/search/2/context.json",
                "https://example.org/autocomplete",
                "http://iiif.io/api/search/2/autocomplete"
            );

            searchService.AddService(autoCompleteService);

            var json = JsonConvert.SerializeObject(searchService, Formatting.Indented);

            // Verify the JSON contains expected properties
            Assert.Contains("@context", json);
            Assert.Contains("@id", json);
            Assert.Contains("@type", json);
            Assert.Contains("profile", json);
            Assert.Contains("service", json);

            // Test round-trip deserialization
            var deserialized = JsonConvert.DeserializeObject<SearchService>(json);
            Assert.NotNull(deserialized);
            Assert.Equal("https://example.org/search", deserialized.Id);
            Assert.Equal("http://iiif.io/api/search/2/search", deserialized.Profile);
            Assert.Single(deserialized.Services);
        }

        [Fact]
        public void AutoCompleteService_SerializesCorrectly()
        {
            var autoCompleteService = new AutoCompleteService(
                "http://iiif.io/api/search/2/context.json",
                "https://example.org/autocomplete",
                "http://iiif.io/api/search/2/autocomplete"
            );

            var json = JsonConvert.SerializeObject(autoCompleteService, Formatting.Indented);

            // Verify the JSON contains expected properties
            Assert.Contains("@context", json);
            Assert.Contains("@id", json);
            Assert.Contains("@type", json);
            Assert.Contains("profile", json);

            // Test round-trip deserialization
            var deserialized = JsonConvert.DeserializeObject<AutoCompleteService>(json);
            Assert.NotNull(deserialized);
            Assert.Equal("https://example.org/autocomplete", deserialized.Id);
            Assert.Equal("http://iiif.io/api/search/2/autocomplete", deserialized.Profile);
        }
    }
}