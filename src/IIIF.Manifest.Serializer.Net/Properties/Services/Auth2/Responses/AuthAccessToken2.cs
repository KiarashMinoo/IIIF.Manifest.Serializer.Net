using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2.Responses;

/// <summary>
///     The successful <c>postMessage</c> response an <see cref="AuthAccessTokenService2" /> sends back
///     to the client - carries the opaque access token the client then sends to a probe service.
/// </summary>
[AuthAPI("2.0")]
public class AuthAccessToken2 : TrackableObject<AuthAccessToken2>
{
    public const string DefaultContext = "http://iiif.io/api/auth/2/context.json";
    public const string ContextJName = "@context";
    public const string TypeJName = "type";
    public const string MessageIdJName = "messageId";
    public const string AccessTokenJName = "accessToken";
    public const string ExpiresInJName = "expiresIn";

    [JsonConstructor]
    public AuthAccessToken2(string messageId, string accessToken)
    {
        Context = DefaultContext;
        Type = "AuthAccessToken2";
        MessageId = messageId;
        AccessToken = accessToken;
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
        get => GetElementValue(x => x.Type) ?? "AuthAccessToken2";
        private set => SetElementValue(value);
    }

    [JsonProperty(MessageIdJName)]
    public string MessageId
    {
        get => GetElementValue(x => x.MessageId)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(AccessTokenJName)]
    public string AccessToken
    {
        get => GetElementValue(x => x.AccessToken)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(ExpiresInJName)]
    public int? ExpiresIn
    {
        get => GetElementValue(x => x.ExpiresIn);
        private set => SetElementValue(value);
    }

    public AuthAccessToken2 SetExpiresIn(int expiresIn)
    {
        ExpiresIn = expiresIn;
        return this;
    }
}