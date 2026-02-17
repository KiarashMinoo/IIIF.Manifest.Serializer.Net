using FluentAssertions;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Service;
using Newtonsoft.Json;
using Xunit;

namespace IIIF.Manifest.Serializer.Net.Tests.Properties
{
    public class AuthService1Tests
    {
        [Fact]
        public void AuthService1_ShouldSerializeWithCorrectProperties()
        {
            // Arrange
            var tokenService = new AuthService1(
                "https://auth.example.org/token",
                Profile.AuthToken.Value
            );

            var logoutService = new AuthService1(
                "https://auth.example.org/logout",
                Profile.AuthLogout.Value
            ).SetLabel("Logout");

            var loginService = new AuthService1(
                "https://auth.example.org/login",
                Profile.AuthLogin.Value
            )
            .SetLabel("Login to Example")
            .SetHeader("Please Log In")
            .SetDescription("This content requires authentication.")
            .SetConfirmLabel("Login")
            .SetFailureHeader("Authentication Failed")
            .SetFailureDescription("Unable to authenticate.")
            .AddService(tokenService)
            .AddService(logoutService);

            // Act
            var json = JsonConvert.SerializeObject(loginService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"@context\": \"http://iiif.io/api/auth/1/context.json\"");
            json.Should().Contain("\"@id\": \"https://auth.example.org/login\"");
            json.Should().Contain("\"profile\": \"http://iiif.io/api/auth/1/login\"");
            json.Should().Contain("\"label\": \"Login to Example\"");
            json.Should().Contain("\"header\": \"Please Log In\"");
            json.Should().Contain("\"description\": \"This content requires authentication.\"");
            json.Should().Contain("\"confirmLabel\": \"Login\"");
            json.Should().Contain("\"failureHeader\": \"Authentication Failed\"");
            json.Should().Contain("\"failureDescription\": \"Unable to authenticate.\"");
            json.Should().Contain("\"service\"");
        }

        [Fact]
        public void AuthService1_ShouldDeserializeCorrectly()
        {
            // Arrange
            var json = @"{
                ""@context"": ""http://iiif.io/api/auth/1/context.json"",
                ""@id"": ""https://auth.example.org/login"",
                ""profile"": ""http://iiif.io/api/auth/1/login"",
                ""label"": ""Login Required"",
                ""header"": ""Authentication Required"",
                ""description"": ""Please log in to view this content."",
                ""confirmLabel"": ""Login"",
                ""failureHeader"": ""Login Failed"",
                ""failureDescription"": ""Authentication was unsuccessful."",
                ""service"": [
                    {
                        ""@context"": ""http://iiif.io/api/auth/1/context.json"",
                        ""@id"": ""https://auth.example.org/token"",
                        ""profile"": ""http://iiif.io/api/auth/1/token""
                    },
                    {
                        ""@context"": ""http://iiif.io/api/auth/1/context.json"",
                        ""@id"": ""https://auth.example.org/logout"",
                        ""profile"": ""http://iiif.io/api/auth/1/logout"",
                        ""label"": ""Logout""
                    }
                ]
            }";

            // Act
            var authService = JsonConvert.DeserializeObject<AuthService1>(json);

            // Assert
            authService.Should().NotBeNull();
            authService.Id.Should().Be("https://auth.example.org/login");
            authService.Profile.Should().Be("http://iiif.io/api/auth/1/login");
            authService.Label.Should().Be("Login Required");
            authService.Header.Should().Be("Authentication Required");
            authService.Description.Should().Be("Please log in to view this content.");
            authService.ConfirmLabel.Should().Be("Login");
            authService.FailureHeader.Should().Be("Login Failed");
            authService.FailureDescription.Should().Be("Authentication was unsuccessful.");
            authService.Services.Should().HaveCount(2);
        }

        [Fact]
        public void AuthService1_ShouldSerializeClickthroughPattern()
        {
            // Arrange
            var tokenService = new AuthService1(
                "https://auth.example.org/clickthrough/token",
                Profile.AuthToken.Value
            );

            var clickthroughService = new AuthService1(
                "https://auth.example.org/clickthrough",
                Profile.AuthClickthrough.Value
            )
            .SetLabel("Terms of Use")
            .SetDescription("By clicking Accept, you agree to the terms.")
            .SetConfirmLabel("Accept")
            .AddService(tokenService);

            // Act
            var json = JsonConvert.SerializeObject(clickthroughService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"profile\": \"http://iiif.io/api/auth/1/clickthrough\"");
            json.Should().Contain("\"label\": \"Terms of Use\"");
            json.Should().Contain("\"confirmLabel\": \"Accept\"");
        }

        [Fact]
        public void AuthService1_ShouldSerializeKioskPattern()
        {
            // Arrange
            var kioskService = new AuthService1(
                "https://auth.example.org/kiosk",
                Profile.AuthKiosk.Value
            )
            .SetLabel("Restricted to Kiosk Access");

            // Act
            var json = JsonConvert.SerializeObject(kioskService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"profile\": \"http://iiif.io/api/auth/1/kiosk\"");
        }

        [Fact]
        public void AuthService1_ShouldSerializeExternalPattern()
        {
            // Arrange
            var tokenService = new AuthService1(
                "https://auth.example.org/external/token",
                Profile.AuthToken.Value
            );

            var externalService = new AuthService1(
                "https://auth.example.org/external",
                Profile.AuthExternal.Value
            )
            .SetLabel("External Authentication")
            .AddService(tokenService);

            // Act
            var json = JsonConvert.SerializeObject(externalService, Formatting.Indented);

            // Assert
            json.Should().Contain("\"profile\": \"http://iiif.io/api/auth/1/external\"");
        }

        [Fact]
        public void AuthService1_RoundTrip_ShouldProduceSameJSON()
        {
            // Arrange
            var originalService = new AuthService1(
                "https://auth.example.org/login",
                Profile.AuthLogin.Value
            )
            .SetLabel("Test Service")
            .SetHeader("Test Header");

            var originalJson = JsonConvert.SerializeObject(originalService, Formatting.Indented);

            // Act
            var deserialized = JsonConvert.DeserializeObject<AuthService1>(originalJson);
            var reserializedJson = JsonConvert.SerializeObject(deserialized, Formatting.Indented);

            // Assert
            reserializedJson.Should().Be(originalJson);
        }

        [Fact]
        public void AuthService1_ShouldHandleSingleServiceNotAsArray()
        {
            // Arrange
            var service = new AuthService1(
                "https://auth.example.org/login",
                Profile.AuthLogin.Value
            );

            var tokenService = new AuthService1(
                "https://auth.example.org/token",
                Profile.AuthToken.Value
            );

            service.AddService(tokenService);

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);

            // Assert - single service should not be wrapped in array
            json.Should().Contain("\"service\": {");
            json.Should().NotContain("\"service\": [");
        }

        [Fact]
        public void AuthService1_ShouldSerializeMultipleServicesAsArray()
        {
            // Arrange
            var service = new AuthService1(
                "https://auth.example.org/login",
                Profile.AuthLogin.Value
            );

            service.AddService(new AuthService1(
                "https://auth.example.org/token",
                Profile.AuthToken.Value
            ));

            service.AddService(new AuthService1(
                "https://auth.example.org/logout",
                Profile.AuthLogout.Value
            ));

            // Act
            var json = JsonConvert.SerializeObject(service, Formatting.Indented);

            // Assert - multiple services should be array
            json.Should().Contain("\"service\": [");
        }
    }
}
