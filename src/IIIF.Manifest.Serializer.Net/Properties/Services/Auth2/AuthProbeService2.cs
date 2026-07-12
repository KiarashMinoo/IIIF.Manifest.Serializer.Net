using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2;

/// <summary>
///     IIIF Authentication Flow API 2.0 - used by the client to check whether the user already has
///     access to the protected resource, without requiring interaction. No "profile" field per spec;
///     wraps one or more <see cref="AuthAccessService2" /> via the inherited <c>Service</c> collection.
/// </summary>
[AuthAPI("2.0", Notes = "Auth API 2.0 probe service.")]
public class AuthProbeService2 : UnprefixedBaseItem<AuthProbeService2>, IBaseService
{
    public const string ErrorHeadingJName = "errorHeading";
    public const string ErrorNoteJName = "errorNote";

    [JsonConstructor]
    public AuthProbeService2(string id, IReadOnlyCollection<IBaseService> service) : base(id, "AuthProbeService2")
    {
        SetService(service);
    }

    public AuthProbeService2(string id, AuthAccessService2 accessService)
        : this(id, (IReadOnlyCollection<IBaseService>)[accessService])
    {
    }

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

    [AuthAPI("2.0")] [JsonIgnore] public IReadOnlyCollection<AuthAccessService2> AccessServices => Service.OfType<AuthAccessService2>().ToList();

    string IBaseService.Profile => string.Empty;

    public AuthProbeService2 AddAccessService(AuthAccessService2 accessService)
    {
        return AddService(accessService);
    }

    public AuthProbeService2 SetErrorHeading(string text)
    {
        return SetElementValue(x => x.ErrorHeading, (IReadOnlyCollection<Label>)[new Label(text)]);
    }

    public AuthProbeService2 SetErrorNote(string text)
    {
        return SetElementValue(x => x.ErrorNote, (IReadOnlyCollection<Label>)[new Label(text)]);
    }
}