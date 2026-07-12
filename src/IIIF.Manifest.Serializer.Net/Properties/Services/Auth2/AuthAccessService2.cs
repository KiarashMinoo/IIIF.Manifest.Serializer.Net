using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2;

/// <summary>
///     IIIF Authentication Flow API 2.0 - the user-interface service opened by the client in a new
///     tab/window. <c>Id</c> is required for the "active"/"kiosk" profiles and must be absent
///     for "external" (handled by <see cref="UnprefixedBaseItem{TBaseItem}" /> accepting a nullable id).
///     <see cref="Label" /> is required for "active". Wraps one required <see cref="AuthAccessTokenService2" />
///     and one optional <see cref="AuthLogoutService2" /> via the inherited <c>Service</c> collection.
/// </summary>
[AuthAPI("2.0", Notes = "Auth API 2.0 access service (active/kiosk/external profiles).")]
public class AuthAccessService2 : UnprefixedBaseItem<AuthAccessService2>, IBaseService
{
    public const string LabelJName = "label";
    public const string HeadingJName = "heading";
    public const string NoteJName = "note";
    public const string ConfirmLabelJName = "confirmLabel";

    [JsonConstructor]
    private AuthAccessService2(string? id, string profile, IReadOnlyCollection<IBaseService> service)
        : base(id, "AuthAccessService2")
    {
        Profile = profile;
        SetService(service);
    }

    /// <summary>
    ///     Creates an "active" or "kiosk" profile access service - both require <paramref name="id" />.
    ///     Use <see cref="ForExternalProfile" /> for the "external" profile, which must omit id.
    /// </summary>
    public AuthAccessService2(string id, string profile, AuthAccessTokenService2 accessTokenService)
        : this((string?)id, profile, (IReadOnlyCollection<IBaseService>)[accessTokenService])
    {
    }

    [AuthAPI("2.0")]
    [JsonProperty(LabelJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    [AuthAPI("2.0")]
    [JsonProperty(HeadingJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Heading
    {
        get => GetElementValue(x => x.Heading) ?? [];
        private set => SetElementValue(value);
    }

    [AuthAPI("2.0")]
    [JsonProperty(NoteJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> Note
    {
        get => GetElementValue(x => x.Note) ?? [];
        private set => SetElementValue(value);
    }

    [AuthAPI("2.0")]
    [JsonProperty(ConfirmLabelJName)]
    [JsonConverter(typeof(LanguageMapJsonConverter))]
    public IReadOnlyCollection<Label> ConfirmLabel
    {
        get => GetElementValue(x => x.ConfirmLabel) ?? [];
        private set => SetElementValue(value);
    }

    [AuthAPI("2.0")] [JsonIgnore] public AuthAccessTokenService2 AccessTokenService => Service.OfType<AuthAccessTokenService2>().Single();

    [AuthAPI("2.0")] [JsonIgnore] public AuthLogoutService2? LogoutService => Service.OfType<AuthLogoutService2>().SingleOrDefault();

    [AuthAPI("2.0")]
    [JsonProperty(IBaseService.ProfileJName)]
    public string Profile
    {
        get => GetElementValue(x => x.Profile)!;
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Creates an "external" profile access service - id must not be present per spec.
    /// </summary>
    public static AuthAccessService2 ForExternalProfile(AuthAccessTokenService2 accessTokenService)
    {
        return new AuthAccessService2(null, "external", (IReadOnlyCollection<IBaseService>)[accessTokenService]);
    }

    public AuthAccessService2 SetLabel(string label)
    {
        return SetElementValue(x => x.Label, (IReadOnlyCollection<Label>)[new Label(label)]);
    }

    public AuthAccessService2 SetHeading(string heading)
    {
        return SetElementValue(x => x.Heading, (IReadOnlyCollection<Label>)[new Label(heading)]);
    }

    public AuthAccessService2 SetNote(string note)
    {
        return SetElementValue(x => x.Note, (IReadOnlyCollection<Label>)[new Label(note)]);
    }

    public AuthAccessService2 SetConfirmLabel(string confirmLabel)
    {
        return SetElementValue(x => x.ConfirmLabel, (IReadOnlyCollection<Label>)[new Label(confirmLabel)]);
    }

    public AuthAccessService2 SetLogoutService(AuthLogoutService2 logoutService)
    {
        return AddService(logoutService);
    }
}