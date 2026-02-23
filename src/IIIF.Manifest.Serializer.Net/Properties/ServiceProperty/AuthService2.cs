using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
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
        public string Profile
        {
            get => GetElementValue(a => a.Profile)!;
            private set => SetElementValue(value);
        }

        /// <summary>
        /// User-facing label for the service (may be language map in full implementation).
        /// </summary>
        public string? Label
        {
            get => GetElementValue(a => a.Label);
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Heading text for the authentication interface.
        /// </summary>
        public string? Heading
        {
            get => GetElementValue(a => a.Heading);
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Explanatory note for the authentication requirement.
        /// </summary>
        public string? Note
        {
            get => GetElementValue(a => a.Note);
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Label for the confirmation button.
        /// </summary>
        public string? ConfirmLabel
        {
            get => GetElementValue(a => a.ConfirmLabel);
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Nested services (e.g., access service within probe, token service within access, logout within token).
        /// </summary>
        public IReadOnlyCollection<AuthService2> Services
        {
            get => GetElementValue(a => a.Services) ?? [];
            private set => SetElementValue(value);
        }

        /// <summary>
        /// Creates a new Auth API 2.0 service.
        /// </summary>
        /// <param name="id">Service identifier</param>
        [JsonConstructor]
        public AuthService2(string id)
            : base(id, string.Empty, "http://iiif.io/api/auth/2/context.json")
        {
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
        public AuthService2 SetProfile(string profile) => SetElementValue(a => a.Profile, profile);

        /// <summary>
        /// Set the label for this service.
        /// </summary>
        public AuthService2 SetLabel(string label) => SetElementValue(a => a.Label, label);

        /// <summary>
        /// Set the heading for the authentication interface.
        /// </summary>
        public AuthService2 SetHeading(string heading) => SetElementValue(a => a.Heading, heading);

        /// <summary>
        /// Set the note explaining the authentication requirement.
        /// </summary>
        public AuthService2 SetNote(string note) => SetElementValue(a => a.Note, note);

        /// <summary>
        /// Set the confirm button label.
        /// </summary>
        public AuthService2 SetConfirmLabel(string confirmLabel) => SetElementValue(a => a.ConfirmLabel, confirmLabel);

        /// <summary>
        /// Add a nested service (access, token, or logout service).
        /// </summary>
        public AuthService2 AddService(AuthService2 service) => SetElementValue(a => a.Services, collection => collection.With(service));

        /// <summary>
        /// Remove a nested service.
        /// </summary>
        public AuthService2 RemoveService(AuthService2 service) => SetElementValue(a => a.Services, collection => collection.Without(service));
    }
}