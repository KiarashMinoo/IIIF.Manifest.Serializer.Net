using System.Linq;
using IIIF.Manifests.Serializer.Properties.Services.Auth2;
using IIIF.Manifests.Serializer.Properties.Services.Auth2.Responses;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Milestone 11 (SDK_VERSIONING_GUIDE.md §10, finding 3): Auth 2.0 was one flat class standing in
/// for 4 distinct service types plus 3 unmodeled response shapes. These tests exercise the 4 real
/// service types directly (not just through the inline-service round trip already covered in
/// ServiceRoundTripTests.cs) and the 3 new response objects (AuthProbeResult2/AuthAccessToken2/
/// AuthAccessTokenError2), which are HTTP/postMessage payloads, not embedded services.
/// </summary>
public class Auth2ServiceTests
{
    [Fact]
    public void AuthProbeService2_Should_HaveNoProfileField()
    {
        var probe = new AuthProbeService2("https://example.org/auth/probe",
            AuthAccessService2.ForExternalProfile(new AuthAccessTokenService2("https://example.org/auth/token")));

        var obj = JObject.Parse(probe.Serialize());

        obj["profile"].Should().BeNull();
        obj["type"]!.ToString().Should().Be("AuthProbeService2");
    }

    [Fact]
    public void AuthProbeService2_Should_RoundTripErrorHeadingAndNote()
    {
        var probe = new AuthProbeService2("https://example.org/auth/probe",
            AuthAccessService2.ForExternalProfile(new AuthAccessTokenService2("https://example.org/auth/token")))
            .SetErrorHeading("Access Denied")
            .SetErrorNote("You do not have permission.");

        var deserialized = TrackableObject.Parse<AuthProbeService2>(probe.Serialize());

        deserialized.ErrorHeading.Single().Value.Should().Be("Access Denied");
        deserialized.ErrorNote.Single().Value.Should().Be("You do not have permission.");
    }

    [Fact]
    public void AuthLogoutService2_Should_RequireLabel()
    {
        var logout = new AuthLogoutService2("https://example.org/auth/logout", "Logout of Example Institution");

        var obj = JObject.Parse(logout.Serialize());

        obj["label"]!["none"]![0]!.ToString().Should().Be("Logout of Example Institution");
        obj["profile"].Should().BeNull();
    }

    [Fact]
    public void AuthAccessTokenService2_Should_RoundTripErrorFields()
    {
        var service = new AuthAccessTokenService2("https://example.org/auth/token")
            .SetErrorHeading("Token error")
            .SetErrorNote("Could not issue a token.");

        var deserialized = TrackableObject.Parse<AuthAccessTokenService2>(service.Serialize());

        deserialized.Id.Should().Be("https://example.org/auth/token");
        deserialized.ErrorHeading.Single().Value.Should().Be("Token error");
        deserialized.ErrorNote.Single().Value.Should().Be("Could not issue a token.");
    }

    [Fact]
    public void AuthProbeResult2_Should_WriteRequiredContextTypeAndStatus()
    {
        var result = new AuthProbeResult2(401)
            .AddSubstitute(new AuthResourceReference("https://example.org/lo-res.mp4", "Video"));

        var obj = JObject.Parse(result.Serialize());

        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/auth/2/context.json");
        obj["type"]!.ToString().Should().Be("AuthProbeResult2");
        obj["status"]!.Value<int>().Should().Be(401);
        obj["substitute"]!["id"]!.ToString().Should().Be("https://example.org/lo-res.mp4");
    }

    [Fact]
    public void AuthProbeResult2_Should_RoundTripLocationHeadingAndNote()
    {
        var result = new AuthProbeResult2(302)
            .SetLocation(new AuthResourceReference("https://example.org/redirected", "Canvas"))
            .SetHeading("Redirecting")
            .SetNote("Please wait");

        var deserialized = TrackableObject.Parse<AuthProbeResult2>(result.Serialize());

        deserialized.Status.Should().Be(302);
        deserialized.Location!.Id.Should().Be("https://example.org/redirected");
        deserialized.Heading.Single().Value.Should().Be("Redirecting");
        deserialized.Note.Single().Value.Should().Be("Please wait");
    }

    [Fact]
    public void AuthAccessToken2_Should_RoundTripThroughSerialize()
    {
        var token = new AuthAccessToken2("ae3415", "ddc76e416e3804e2369e6c9cee806f5e438a5cdc").SetExpiresIn(300);

        var obj = JObject.Parse(token.Serialize());
        obj["@context"]!.ToString().Should().Be("http://iiif.io/api/auth/2/context.json");
        obj["type"]!.ToString().Should().Be("AuthAccessToken2");

        var deserialized = TrackableObject.Parse<AuthAccessToken2>(token.Serialize());
        deserialized.MessageId.Should().Be("ae3415");
        deserialized.AccessToken.Should().Be("ddc76e416e3804e2369e6c9cee806f5e438a5cdc");
        deserialized.ExpiresIn.Should().Be(300);
    }

    [Fact]
    public void AuthAccessTokenError2_Should_RoundTripThroughSerialize()
    {
        var error = new AuthAccessTokenError2(AuthAccessTokenError2.MissingAspect, "ae3415")
            .SetHeading("Unauthorized")
            .SetNote("Call Bob at ext. 1234 to refill the cookie jar");

        var deserialized = TrackableObject.Parse<AuthAccessTokenError2>(error.Serialize());

        deserialized.Profile.Should().Be("missingAspect");
        deserialized.MessageId.Should().Be("ae3415");
        deserialized.Heading.Single().Value.Should().Be("Unauthorized");
    }
}
