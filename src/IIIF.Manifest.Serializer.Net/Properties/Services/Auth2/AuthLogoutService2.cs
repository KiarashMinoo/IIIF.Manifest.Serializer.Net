using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2;

/// <summary>
///     IIIF Authentication Flow API 2.0 - lets the client close the user's session (e.g. on a public
///     terminal, or to log in again as a different account). No "profile" field per spec; "label" is
///     required and must clearly identify the domain/institution the user is logging out of.
/// </summary>
[AuthAPI("2.0", Notes = "Auth API 2.0 logout service.")]
public class AuthLogoutService2 : UnprefixedBaseItem<AuthLogoutService2>, IBaseService
{
    public const string LabelJName = "label";

    [JsonConstructor]
    public AuthLogoutService2(string id, IReadOnlyCollection<Label> label) : base(id, "AuthLogoutService2")
    {
        Label = label;
    }

    public AuthLogoutService2(string id, string label) : this(id, (IReadOnlyCollection<Label>)[new Label(label)])
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

    string IBaseService.Profile => string.Empty;
}