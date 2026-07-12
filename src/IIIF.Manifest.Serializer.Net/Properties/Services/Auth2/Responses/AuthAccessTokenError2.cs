using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2.Responses;

/// <summary>
///     The error <c>postMessage</c> response an <see cref="AuthAccessTokenService2" /> sends back to
///     the client when it cannot issue an access token. <see cref="Profile" /> is one of the 6 error
///     codes the spec defines (invalidRequest/invalidOrigin/missingAspect/invalidAspect/expiredAspect/
///     unavailable) - modeled as a plain string rather than an enum since it is spec-open-ended text.
/// </summary>
[AuthAPI("2.0")]
public class AuthAccessTokenError2 : TrackableObject<AuthAccessTokenError2>
{
    public const string DefaultContext = "http://iiif.io/api/auth/2/context.json";
    public const string ContextJName = "@context";
    public const string TypeJName = "type";
    public const string ProfileJName = "profile";
    public const string MessageIdJName = "messageId";
    public const string HeadingJName = "heading";
    public const string NoteJName = "note";

    public const string InvalidRequest = "invalidRequest";
    public const string InvalidOrigin = "invalidOrigin";
    public const string MissingAspect = "missingAspect";
    public const string InvalidAspect = "invalidAspect";
    public const string ExpiredAspect = "expiredAspect";
    public const string Unavailable = "unavailable";

    [JsonConstructor]
    public AuthAccessTokenError2(string profile, string messageId)
    {
        Context = DefaultContext;
        Type = "AuthAccessTokenError2";
        Profile = profile;
        MessageId = messageId;
    }

    [JsonProperty(ContextJName)]
    public string Context
    {
        get => GetElementValue(x => x.Context) ?? DefaultContext;
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "AuthAccessTokenError2";
        private set => SetElementValue(value);
    }

    [JsonProperty(ProfileJName)]
    public string Profile
    {
        get => GetElementValue(x => x.Profile)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(MessageIdJName)]
    public string MessageId
    {
        get => GetElementValue(x => x.MessageId)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(HeadingJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Heading
    {
        get => GetElementValue(x => x.Heading) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(NoteJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Note
    {
        get => GetElementValue(x => x.Note) ?? [];
        private set => SetElementValue(value);
    }

    public AuthAccessTokenError2 SetHeading(string heading)
    {
        return SetElementValue(x => x.Heading, (IReadOnlyCollection<Label>)[new Label(heading)]);
    }

    public AuthAccessTokenError2 SetNote(string note)
    {
        return SetElementValue(x => x.Note, (IReadOnlyCollection<Label>)[new Label(note)]);
    }
}