using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services;

/// <summary>
/// IIIF Authentication API 2.0 Service - provides probe and access service patterns.
/// Used for active and external authentication patterns with improved user experience.
/// </summary>
[AuthAPI("2.0", Notes = "Auth API 2.0 service with probe/access patterns.")]
public class AuthService2 : BaseItem<AuthService2>, IBaseService
{
    public const string ProfileJName = "profile";
    public const string LabelJName = "label";
    public const string HeadingJName = "heading";
    public const string NoteJName = "note";
    public const string ConfirmLabelJName = "confirmLabel";

    /// <summary>
    /// The profile of the authentication service (active or external for access services).
    /// </summary>
    [AuthAPI("2.0")]
    [JsonProperty(ProfileJName)]
    public string Profile
    {
        get => GetElementValue(a => a.Profile)!;
        private set => SetElementValue(value);
    }

    /// <summary>
    /// User-facing label for the service (may be language map in full implementation).
    /// </summary>
    [AuthAPI("2.0")]
    [JsonProperty(LabelJName)]
    public string? Label
    {
        get => GetElementValue(a => a.Label);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Heading text for the authentication interface.
    /// </summary>
    [AuthAPI("2.0")]
    [JsonProperty(HeadingJName)]
    public string? Heading
    {
        get => GetElementValue(a => a.Heading);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Explanatory note for the authentication requirement.
    /// </summary>
    [AuthAPI("2.0")]
    [JsonProperty(NoteJName)]
    public string? Note
    {
        get => GetElementValue(a => a.Note);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Label for the confirmation button.
    /// </summary>
    [AuthAPI("2.0")]
    [JsonProperty(ConfirmLabelJName)]
    public string? ConfirmLabel
    {
        get => GetElementValue(a => a.ConfirmLabel);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Creates a new Auth API 2.0 service with a profile.
    /// </summary>
    /// <param name="id">Service identifier</param>
    /// <param name="profile">Auth profile (active or external)</param>
    public AuthService2(string id, string profile)
        : base(id, string.Empty, "http://iiif.io/api/auth/2/context.json")
    {
        Profile = profile;
    }

    /// <summary>
    /// Set the profile for this authentication service.
    /// </summary>
    public AuthService2 SetProfile(string profile)
    {
        Profile = profile;
        return this;
    }

    /// <summary>
    /// Set the label for this service.
    /// </summary>
    public AuthService2 SetLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    /// Set the heading for the authentication interface.
    /// </summary>
    public AuthService2 SetHeading(string heading)
    {
        Heading = heading;
        return this;
    }

    /// <summary>
    /// Set the note explaining the authentication requirement.
    /// </summary>
    public AuthService2 SetNote(string note)
    {
        Note = note;
        return this;
    }

    /// <summary>
    /// Set the confirm button label.
    /// </summary>
    public AuthService2 SetConfirmLabel(string confirmLabel)
    {
        ConfirmLabel = confirmLabel;
        return this;
    }
}