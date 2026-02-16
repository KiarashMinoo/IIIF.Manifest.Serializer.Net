using IIIF.Manifests.Serializer.Nodes.Collection;
using IIIF.Manifests.Serializer.Properties;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Nodes
{
    public class CollectionTests
    {
        [Fact]
        public void Collection_ShouldSerializeWithManifests()
        {
            // Arrange
            var collection = new Collection(
                "https://example.org/collection/1",
                new Label("Test Collection")
            );

            collection.AddManifest("https://example.org/manifest/1");
            collection.AddManifest("https://example.org/manifest/2");
            collection.AddManifest("https://example.org/manifest/3");

            // Act
            var json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Collection>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Manifests.Should().HaveCount(3);
            deserialized.Label.Should().ContainSingle();
        }

        [Fact]
        public void Collection_ShouldSupportNestedCollections()
        {
            // Arrange
            var subCollection1 = new Collection(
                "https://example.org/collection/sub1",
                new Label("Sub Collection 1")
            );
            subCollection1.AddManifest("https://example.org/manifest/1");

            var subCollection2 = new Collection(
                "https://example.org/collection/sub2",
                new Label("Sub Collection 2")
            );
            subCollection2.AddManifest("https://example.org/manifest/2");

            var parentCollection = new Collection(
                "https://example.org/collection/parent",
                new Label("Parent Collection")
            );
            parentCollection.AddCollection(subCollection1);
            parentCollection.AddCollection(subCollection2);

            // Act
            var json = JsonConvert.SerializeObject(parentCollection, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Collection>(json);

            // Assert
            deserialized.Collections.Should().HaveCount(2);
        }

        [Fact]
        public void Collection_ShouldSupportTotal()
        {
            // Arrange
            var collection = new Collection(
                "https://example.org/collection/large",
                new Label("Large Collection")
            );
            collection.SetTotal(1000);
            collection.AddManifest("https://example.org/manifest/1");

            // Act
            var json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Collection>(json);

            // Assert
            deserialized.Total.Should().Be(1000);
        }

        [Fact]
        public void Collection_ShouldSupportViewingDirection()
        {
            // Arrange
            var collection = new Collection(
                "https://example.org/collection/1",
                new Label("Right-to-Left Collection")
            );
            collection.SetViewingDirection(ViewingDirection.Rtl);

            // Act
            var json = JsonConvert.SerializeObject(collection, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Collection>(json);

            // Assert
            deserialized.ViewingDirection.Value.Should().Be("right-to-left");
        }
    }
}

