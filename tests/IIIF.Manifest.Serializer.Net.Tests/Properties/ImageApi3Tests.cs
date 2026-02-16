using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Service;
using IIIF.Manifests.Serializer.Properties.Size;
using IIIF.Manifests.Serializer.Properties.Tile;
using FluentAssertions;
using Newtonsoft.Json;
using System.Linq;
using Xunit;
namespace IIIF.Manifests.Serializer.Tests.Properties
{
    public class ImageApi3Tests
    {
        [Fact]
        public void Service_ShouldSupportSizes()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.AddSize(new Size(150, 100));
            service.AddSize(new Size(600, 400));
            service.AddSize(new Size(1200, 800));
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.Sizes.Should().HaveCount(3);
            deserialized.Sizes.Should().Contain(s => s.Width == 150 && s.Height == 100);
            deserialized.Sizes.Should().Contain(s => s.Width == 600 && s.Height == 400);
            deserialized.Sizes.Should().Contain(s => s.Width == 1200 && s.Height == 800);
        }
        [Fact]
        public void Service_ShouldSupportMaxDimensions()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.SetMaxWidth(2000);
            service.SetMaxHeight(1500);
            service.SetMaxArea(10000000);
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.MaxWidth.Should().Be(2000);
            deserialized.MaxHeight.Should().Be(1500);
            deserialized.MaxArea.Should().Be(10000000);
        }
        [Fact]
        public void Service_ShouldSupportRights()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.SetRights(Rights.CcBy);
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.Rights.Should().Be("http://creativecommons.org/licenses/by/4.0/");
        }
        [Fact]
        public void Service_ShouldSupportPreferredFormatsWithValueObjects()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.AddPreferredFormat(ImageFormat.Webp);
            service.AddPreferredFormat(ImageFormat.Jpg);
            service.AddPreferredFormat(ImageFormat.Png);
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.PreferredFormats.Should().HaveCount(3);
            deserialized.PreferredFormats.Select(f => f.Value).Should().Contain("webp");
            deserialized.PreferredFormats.Select(f => f.Value).Should().Contain("jpg");
            deserialized.PreferredFormats.Select(f => f.Value).Should().Contain("png");
        }
        [Fact]
        public void Service_ShouldSupportExtraQualitiesWithValueObjects()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.AddExtraQuality(ImageQuality.Bitonal);
            service.AddExtraQuality(ImageQuality.Gray);
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.ExtraQualities.Should().HaveCount(2);
            deserialized.ExtraQualities.Select(q => q.Value).Should().Contain("bitonal");
            deserialized.ExtraQualities.Select(q => q.Value).Should().Contain("gray");
        }
        [Fact]
        public void Service_ShouldSupportExtraFeaturesWithValueObjects()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.AddExtraFeature(ImageFeature.RegionByPx);
            service.AddExtraFeature(ImageFeature.SizeByW);
            service.AddExtraFeature(ImageFeature.RotationArbitrary);
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.ExtraFeatures.Should().HaveCount(3);
            deserialized.ExtraFeatures.Select(f => f.Value).Should().Contain("regionByPx");
            deserialized.ExtraFeatures.Select(f => f.Value).Should().Contain("sizeByW");
            deserialized.ExtraFeatures.Select(f => f.Value).Should().Contain("rotationArbitrary");
        }
        [Fact]
        public void Service_ShouldSupportAllImageApi3PropertiesWithValueObjects()
        {
            // Arrange - Create a full Image API 3.0 service using value objects
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            service.SetHeight(4000)
                   .SetWidth(3000)
                   .SetMaxWidth(2000)
                   .SetMaxHeight(1500)
                   .SetMaxArea(10000000)
                   .SetRights(Rights.CcBy)
                   .AddSize(new Size(150, 100))
                   .AddSize(new Size(600, 400))
                   .AddPreferredFormat(ImageFormat.Webp)
                   .AddPreferredFormat(ImageFormat.Jpg)
                   .AddExtraQuality(ImageQuality.Bitonal)
                   .AddExtraFeature(ImageFeature.RegionByPx);
            var tile = new Tile().SetWidth(512);
            tile.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8);
            service.AddTile(tile);
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.Should().NotBeNull();
            deserialized.Height.Should().Be(4000);
            deserialized.Width.Should().Be(3000);
            deserialized.MaxWidth.Should().Be(2000);
            deserialized.MaxHeight.Should().Be(1500);
            deserialized.MaxArea.Should().Be(10000000);
            deserialized.Rights.Should().Be("http://creativecommons.org/licenses/by/4.0/");
            deserialized.Sizes.Should().HaveCount(2);
            deserialized.Tiles.Should().HaveCount(1);
            deserialized.PreferredFormats.Should().HaveCount(2);
            deserialized.ExtraQualities.Should().ContainSingle();
            deserialized.ExtraFeatures.Should().ContainSingle();
        }
        [Fact]
        public void Service_ShouldAcceptBothStringAndValueObjectOverloads()
        {
            // Arrange
            var service = new Service(
                "http://iiif.io/api/image/3/context.json",
                "https://example.org/iiif/image1",
                "level2"
            );
            // Mix of value objects and strings
            service.AddPreferredFormat(ImageFormat.Webp);
            service.AddPreferredFormat("avif"); // String overload for custom format
            service.AddExtraQuality(ImageQuality.Bitonal);
            service.AddExtraQuality("custom-quality"); // String for custom
            service.AddExtraFeature(ImageFeature.Cors);
            service.AddExtraFeature("custom-feature"); // String for custom
            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<Service>(json);
            // Assert
            deserialized.PreferredFormats.Should().HaveCount(2);
            deserialized.ExtraQualities.Should().HaveCount(2);
            deserialized.ExtraFeatures.Should().HaveCount(2);
        }
    }
}
