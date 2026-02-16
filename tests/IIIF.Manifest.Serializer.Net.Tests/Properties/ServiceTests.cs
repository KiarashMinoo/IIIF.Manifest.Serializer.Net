using IIIF.Manifests.Serializer.Nodes.Content.Image.Resource;
using IIIF.Manifests.Serializer.Properties.Service;
using IIIF.Manifests.Serializer.Properties.Tile;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class ServiceTests
    {
        [Fact]
        public void Service_ShouldSerializeBasicProperties()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://example.org/iiif/image1",
                "http://iiif.io/api/image/2/level1.json"
            );

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Id.Should().Be("https://example.org/iiif/image1");
            deserialized.Profile.Should().Be("http://iiif.io/api/image/2/level1.json");
        }

        [Fact]
        public void Service_ShouldSupportDimensions()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://example.org/iiif/image1",
                "http://iiif.io/api/image/2/level2.json"
            )
            .SetHeight(2000)
            .SetWidth(1500);

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);

            // Assert
            deserialized.Height.Should().Be(2000);
            deserialized.Width.Should().Be(1500);
        }

        [Fact]
        public void Service_ShouldSupportTiles()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://example.org/iiif/image1",
                "http://iiif.io/api/image/2/level2.json"
            );
            
            var tile1 = new Tile().SetWidth(512);
            tile1.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8);
            
            var tile2 = new Tile().SetWidth(1024);
            tile2.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4);
            
            service.AddTile(tile1).AddTile(tile2);

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);

            // Assert
            deserialized.Tiles.Should().HaveCount(2);
            deserialized.Tiles.Should().Contain(t => t.Width == 512);
            deserialized.Tiles.Should().Contain(t => t.Width == 1024);
        }

        [Fact]
        public void ImageResource_ShouldSupportService()
        {
            // Arrange
            var resource = new ImageResource(
                "https://example.org/image.jpg",
                "image/jpeg"
            )
            .SetHeight(2000)
            .SetWidth(1500);

            var service = new Service(
                "http://iiif.io/api/image/2/context.json",
                "https://example.org/iiif/image1",
                "http://iiif.io/api/image/2/level1.json"
            );
            
            var tile = new Tile().SetWidth(512);
            tile.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8);
            service.AddTile(tile);

            resource.SetService(service);

            // Act
            var json = JsonConvert.SerializeObject(resource, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<ImageResource>(json);

            // Assert
            deserialized.Service.Should().NotBeNull();
            deserialized.Service.Profile.Should().Be("http://iiif.io/api/image/2/level1.json");
            deserialized.Service.Tiles.Should().ContainSingle();
        }
    }
}

