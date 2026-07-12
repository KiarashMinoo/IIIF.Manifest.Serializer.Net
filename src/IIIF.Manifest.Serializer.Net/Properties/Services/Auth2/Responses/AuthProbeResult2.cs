using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services.Auth2.Responses;

/// <summary>
///     The JSON body an <see cref="AuthProbeService2" /> HTTP response returns. The outer HTTP status
///     code is always 200; <see cref="Status" /> carries the "real" access status the client acts on.
/// </summary>
[AuthAPI("2.0")]
public class AuthProbeResult2 : TrackableObject<AuthProbeResult2>
{
    public const string DefaultContext = "http://iiif.io/api/auth/2/context.json";
    public const string ContextJName = "@context";
    public const string TypeJName = "type";
    public const string StatusJName = "status";
    public const string SubstituteJName = "substitute";
    public const string LocationJName = "location";
    public const string HeadingJName = "heading";
    public const string NoteJName = "note";

    public AuthProbeResult2(int status)
    {
        Context = DefaultContext;
        Type = "AuthProbeResult2";
        Status = status;
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
        get => GetElementValue(x => x.Type) ?? "AuthProbeResult2";
        private set => SetElementValue(value);
    }

    [JsonProperty(StatusJName)]
    public int Status
    {
        get => GetElementValue(x => x.Status);
        private set => SetElementValue(value);
    }

    [JsonProperty(SubstituteJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<AuthResourceReference> Substitute
    {
        get => GetElementValue(x => x.Substitute) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(LocationJName)]
    public AuthResourceReference? Location
    {
        get => GetElementValue(x => x.Location);
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

    public AuthProbeResult2 AddSubstitute(AuthResourceReference substitute)
    {
        Substitute = Substitute.With(substitute);
        return this;
    }

    public AuthProbeResult2 SetLocation(AuthResourceReference location)
    {
        Location = location;
        return this;
    }

    public AuthProbeResult2 SetHeading(string heading)
    {
        return SetElementValue(x => x.Heading, (IReadOnlyCollection<Label>)[new Label(heading)]);
    }

    public AuthProbeResult2 SetNote(string note)
    {
        return SetElementValue(x => x.Note, (IReadOnlyCollection<Label>)[new Label(note)]);
    }
}