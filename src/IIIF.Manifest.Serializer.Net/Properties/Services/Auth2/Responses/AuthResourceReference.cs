using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2.Responses;

/// <summary>
/// A plain <c>{"id","type"}</c> resource reference, used by <see cref="AuthProbeResult2"/> for its
/// optional <c>substitute</c> and <c>location</c> fields.
/// </summary>
[AuthAPI("2.0")]
public class AuthResourceReference : TrackableObject<AuthResourceReference>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";

    [JsonProperty(IdJName)]
    public string Id
    {
        get => GetElementValue(x => x.Id)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type)!;
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public AuthResourceReference(string id, string type)
    {
        Id = id;
        Type = type;
    }
}
