using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2;

/// <summary>
/// IIIF Authentication Flow API 2.0 - the service the client calls to obtain an access token,
/// which it then sends to a probe service. Has no "profile" field per spec.
/// </summary>
[AuthAPI("2.0", Notes = "Auth API 2.0 access token service.")]
public class AuthAccessTokenService2 : UnprefixedBaseItem<AuthAccessTokenService2>, IBaseService
{
    public const string ErrorHeadingJName = "errorHeading";
    public const string ErrorNoteJName = "errorNote";

    // No "profile" field in the Auth 2.0 spec for this service - satisfy IBaseService without
    // exposing a public/serializable "profile" property on the concrete type.
    string IBaseService.Profile => string.Empty;

    [AuthAPI("2.0")]
    [JsonProperty(ErrorHeadingJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> ErrorHeading
    {
        get => GetElementValue(x => x.ErrorHeading) ?? [];
        private set => SetElementValue(value);
    }

    [AuthAPI("2.0")]
    [JsonProperty(ErrorNoteJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> ErrorNote
    {
        get => GetElementValue(x => x.ErrorNote) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public AuthAccessTokenService2(string id) : base(id, "AuthAccessTokenService2")
    {
    }

    public AuthAccessTokenService2 SetErrorHeading(string text) => SetElementValue(x => x.ErrorHeading, (IReadOnlyCollection<Label>)[new Label(text)]);
    public AuthAccessTokenService2 SetErrorNote(string text) => SetElementValue(x => x.ErrorNote, (IReadOnlyCollection<Label>)[new Label(text)]);
}
