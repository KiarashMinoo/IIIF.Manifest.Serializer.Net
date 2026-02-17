using System.Linq;
using FluentAssertions;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Service;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifest.Serializer.Net.Tests.Properties
{
    public class AuthService2Tests
    {
        [Fact]
        public void AuthService2_ShouldSerializeActivePattern()
        {
            // Arrange
            var logoutService = new AuthService2("https://auth.example.org/logout");

            var tokenService = new AuthService2("https://auth.example.org/token")
                .AddService(logoutService);

            var accessService = new AuthService2(
                "https://auth.example.org/access",
                "active"
            )
            .SetLabel("Login to Access")
            .SetHeading("Authentication Required")
            .SetNote("Please log in to view this content.")
            .SetConfirmLabel("Login")
            .AddService(tokenService);

            var probeService = new AuthService2("https://auth.example.org/probe")
                .AddService(accessService);

            // Act
            var json = JsonConvert.SerializeObject(probeService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"@context\": \"http://iiif.io/api/auth/2/context.json\"");
            json.Should().Contain("\"@id\": \"https://auth.example.org/probe\"");
            json.Should().Contain("\"service\"");
        }

        [Fact]
        public void AuthService2_ShouldDeserializeCorrectly()
        {
            // Arrange
            var json = @"{
                ""@context"": ""http://iiif.io/api/auth/2/context.json"",
                ""@id"": ""https://auth.example.org/access"",
                ""profile"": ""active"",
                ""label"": ""Login Required"",
                ""heading"": ""Authentication Required"",
                ""note"": ""Please log in to view this content."",
                ""confirmLabel"": ""Login""
            }";

            // Act
            var authService = JsonConvert.DeserializeObject<AuthService2>(json);

            // Assert
            authService.Should().NotBeNull();
            authService.Id.Should().Be("https://auth.example.org/access");
            authService.Profile.Should().Be("active");
            authService.Label.Should().Be("Login Required");
            authService.Heading.Should().Be("Authentication Required");
            authService.Note.Should().Be("Please log in to view this content.");
            authService.ConfirmLabel.Should().Be("Login");
        }

        [Fact]
        public void AuthService2_ShouldSerializeExternalPattern()
        {
            // Arrange
            var accessService = new AuthService2(
                "https://auth.example.org/access",
                "external"
            )
            .SetLabel("External Login")
            .SetHeading("External Authentication")
            .SetNote("You will be redirected to an external login page.");

            // Act
            var json = JsonConvert.SerializeObject(accessService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"profile\": \"external\"");
            json.Should().Contain("\"label\": \"External Login\"");
            json.Should().Contain("\"heading\": \"External Authentication\"");
        }

        [Fact]
        public void AuthService2_RoundTrip_ShouldProduceSameJSON()
        {
            // Arrange
            var originalService = new AuthService2(
                "https://auth.example.org/probe"
            )
            .SetLabel("Test Service")
            .SetHeading("Test Heading")
            .SetNote("Test note");

            var originalJson = JsonConvert.SerializeObject(originalService, Formatting.Indented);

            // Act
            var deserialized = JsonConvert.DeserializeObject<AuthService2>(originalJson);
            var reserializedJson = JsonConvert.SerializeObject(deserialized, Formatting.Indented);

            // Assert
            reserializedJson.Should().Be(originalJson);
        }

        [Fact]
        public void AuthService2_ShouldSerializeNestedServiceStructure()
        {
            // Arrange
            var logoutService = new AuthService2("https://auth.example.org/logout");

            var tokenService = new AuthService2("https://auth.example.org/token")
                .AddService(logoutService);

            var accessService = new AuthService2(
                "https://auth.example.org/access",
                "active"
            )
            .AddService(tokenService);

            var probeService = new AuthService2("https://auth.example.org/probe")
                .AddService(accessService);

            // Act
            var json = JsonConvert.SerializeObject(probeService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"@id\": \"https://auth.example.org/probe\"");
            json.Should().Contain("\"@id\": \"https://auth.example.org/access\"");
            json.Should().Contain("\"@id\": \"https://auth.example.org/token\"");
            json.Should().Contain("\"@id\": \"https://auth.example.org/logout\"");
        }

        [Fact]
        public void AuthService2_ShouldHandleSingleServiceNotAsArray()
        {
            // Arrange
            var accessService = new AuthService2("https://auth.example.org/access");
            var tokenService = new AuthService2("https://auth.example.org/token");

            accessService.AddService(tokenService);

            // Act
            var json = JsonConvert.SerializeObject(accessService, Formatting.Indented);

            // Assert - single service should not be wrapped in array
            json.Should().Contain("\"service\": {");
            json.Should().NotContain("\"service\": [");
        }

        [Fact]
        public void AuthService2_ShouldSerializeMultipleServicesAsArray()
        {
            // Arrange
            var service = new AuthService2("https://auth.example.org/probe");

            service.AddService(new AuthService2("https://auth.example.org/access1"));
            service.AddService(new AuthService2("https://auth.example.org/access2"));

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);

            // Assert - multiple services should be array
            json.Should().Contain("\"service\": [");
        }

        [Fact]
        public void AuthService2_ShouldOmitEmptyOptionalFields()
        {
            // Arrange
            var service = new AuthService2("https://auth.example.org/probe");

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);

            // Assert - should not include empty optional fields
            json.Should().NotContain("\"label\"");
            json.Should().NotContain("\"heading\"");
            json.Should().NotContain("\"note\"");
            json.Should().NotContain("\"confirmLabel\"");
            json.Should().NotContain("\"profile\"");
            json.Should().NotContain("\"service\"");
        }

        [Fact]
        public void AuthService2_WithProfile_ShouldSerializeProfile()
        {
            // Arrange
            var service = new AuthService2(
                "https://auth.example.org/access",
                "active"
            );

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);

            // Assert
            json.Should().Contain("\"profile\": \"active\"");
        }

        [Fact]
        public void AuthService2_DeserializeComplexStructure()
        {
            // Arrange
            var json = @"{
                ""@context"": ""http://iiif.io/api/auth/2/context.json"",
                ""@id"": ""https://auth.example.org/probe"",
                ""service"": {
                    ""@context"": ""http://iiif.io/api/auth/2/context.json"",
                    ""@id"": ""https://auth.example.org/access"",
                    ""profile"": ""active"",
                    ""label"": ""Login"",
                    ""service"": {
                        ""@context"": ""http://iiif.io/api/auth/2/context.json"",
                        ""@id"": ""https://auth.example.org/token"",
                        ""service"": {
                            ""@context"": ""http://iiif.io/api/auth/2/context.json"",
                            ""@id"": ""https://auth.example.org/logout""
                        }
                    }
                }
            }";

            // Act
            var authService = JsonConvert.DeserializeObject<AuthService2>(json);

            // Assert
            authService.Should().NotBeNull();
            authService.Id.Should().Be("https://auth.example.org/probe");
            authService.Services.Should().HaveCount(1);
            
            var accessService = authService.Services.First();
            accessService.Id.Should().Be("https://auth.example.org/access");
            accessService.Profile.Should().Be("active");
            accessService.Services.Should().HaveCount(1);

            var tokenService = accessService.Services.First();
            tokenService.Id.Should().Be("https://auth.example.org/token");
            tokenService.Services.Should().HaveCount(1);

            var logoutService = tokenService.Services.First();
            logoutService.Id.Should().Be("https://auth.example.org/logout");
        }
    }
}
