using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    /// <summary>
    /// IIIF Authentication API 2.0 Service - provides probe and access service patterns.
    /// Used for active and external authentication patterns with improved user experience.
    /// </summary>
    [AuthAPI("2.0", Notes = "Auth API 2.0 service with probe/access patterns.")]
    [JsonConverter(typeof(AuthService2JsonConverter))]
    public class AuthService2 : BaseItem<AuthService2>
    {
        public const string ProfileJName = "profile";
        public const string LabelJName = "label";
        public const string HeadingJName = "heading";
        public const string NoteJName = "note";
        public const string ConfirmLabelJName = "confirmLabel";

        private readonly List<AuthService2> _services = new List<AuthService2>();

        /// <summary>
        /// The profile of the authentication service (active or external for access services).
        /// </summary>
        public string Profile { get; private set; }

        /// <summary>
        /// User-facing label for the service (may be language map in full implementation).
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Heading text for the authentication interface.
        /// </summary>
        public string Heading { get; private set; }

        /// <summary>
        /// Explanatory note for the authentication requirement.
        /// </summary>
        public string Note { get; private set; }

        /// <summary>
        /// Label for the confirmation button.
        /// </summary>
        public string ConfirmLabel { get; private set; }

        /// <summary>
        /// Nested services (e.g., access service within probe, token service within access, logout within token).
        /// </summary>
        public IReadOnlyCollection<AuthService2> Services => _services.AsReadOnly();

        /// <summary>
        /// Creates a new Auth API 2.0 service.
        /// </summary>
        /// <param name="id">Service identifier</param>
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
        public AuthService2 SetProfile(string profile) => SetPropertyValue(a => a.Profile, profile);

        /// <summary>
        /// Set the label for this service.
        /// </summary>
        public AuthService2 SetLabel(string label) => SetPropertyValue(a => a.Label, label);

        /// <summary>
        /// Set the heading for the authentication interface.
        /// </summary>
        public AuthService2 SetHeading(string heading) => SetPropertyValue(a => a.Heading, heading);

        /// <summary>
        /// Set the note explaining the authentication requirement.
        /// </summary>
        public AuthService2 SetNote(string note) => SetPropertyValue(a => a.Note, note);

        /// <summary>
        /// Set the confirm button label.
        /// </summary>
        public AuthService2 SetConfirmLabel(string confirmLabel) => SetPropertyValue(a => a.ConfirmLabel, confirmLabel);

        /// <summary>
        /// Add a nested service (access, token, or logout service).
        /// </summary>
        public AuthService2 AddService(AuthService2 service) => SetPropertyValue(a => a._services, a => a.Services, _services.Attach(service));

        /// <summary>
        /// Remove a nested service.
        /// </summary>
        public AuthService2 RemoveService(AuthService2 service) => SetPropertyValue(a => a._services, a => a.Services, _services.Detach(service));
    }
}
