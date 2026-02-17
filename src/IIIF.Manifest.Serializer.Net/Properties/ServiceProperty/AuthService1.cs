using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    /// <summary>
    /// IIIF Authentication API 1.0 Service - provides authentication flow information.
    /// Used for login, clickthrough, kiosk, and external authentication patterns.
    /// </summary>
    [AuthAPI("1.0", Notes = "Auth API 1.0 service for content access control.")]
    [JsonConverter(typeof(AuthService1JsonConverter))]
    public class AuthService1 : BaseItem<AuthService1>, IBaseService
    {
        public const string LabelJName = "label";
        public const string HeaderJName = "header";
        public const string DescriptionJName = "description";
        public const string ConfirmLabelJName = "confirmLabel";
        public const string FailureHeaderJName = "failureHeader";
        public const string FailureDescriptionJName = "failureDescription";

        private readonly List<AuthService1> services = new List<AuthService1>();

        [AuthAPI("1.0")]
        [JsonProperty(IBaseService.ProfileJName)]
        public string Profile { get; }

        [AuthAPI("1.0")]
        [JsonProperty(LabelJName)]
        public string Label { get; private set; }

        [AuthAPI("1.0")]
        [JsonProperty(HeaderJName)]
        public string Header { get; private set; }

        [AuthAPI("1.0")]
        [JsonProperty(DescriptionJName)]
        public string Description { get; private set; }

        [AuthAPI("1.0")]
        [JsonProperty(ConfirmLabelJName)]
        public string ConfirmLabel { get; private set; }

        [AuthAPI("1.0")]
        [JsonProperty(FailureHeaderJName)]
        public string FailureHeader { get; private set; }

        [AuthAPI("1.0")]
        [JsonProperty(FailureDescriptionJName)]
        public string FailureDescription { get; private set; }

        [AuthAPI("1.0")]
        [JsonProperty(ServiceJName)]
        public IReadOnlyCollection<AuthService1> Services => services.AsReadOnly();

        /// <summary>
        /// Creates a new Auth API 1.0 service.
        /// </summary>
        /// <param name="id">Service identifier (login/token/logout endpoint URL)</param>
        /// <param name="profile">Auth profile (login/clickthrough/kiosk/external/token/logout)</param>
        public AuthService1(string id, string profile)
            : base(id, string.Empty, "http://iiif.io/api/auth/1/context.json")
        {
            Profile = profile;
        }

        /// <summary>
        /// Sets the user-facing label for the authentication interaction.
        /// </summary>
        public AuthService1 SetLabel(string label) => SetPropertyValue(a => a.Label, label);

        /// <summary>
        /// Sets the header text for the authentication interaction window.
        /// </summary>
        public AuthService1 SetHeader(string header) => SetPropertyValue(a => a.Header, header);

        /// <summary>
        /// Sets the description text explaining the authentication requirement.
        /// </summary>
        public AuthService1 SetDescription(string description) => SetPropertyValue(a => a.Description, description);

        /// <summary>
        /// Sets the label for the confirm button in clickthrough interactions.
        /// </summary>
        public AuthService1 SetConfirmLabel(string confirmLabel) => SetPropertyValue(a => a.ConfirmLabel, confirmLabel);

        /// <summary>
        /// Sets the header text shown when authentication fails.
        /// </summary>
        public AuthService1 SetFailureHeader(string failureHeader) => SetPropertyValue(a => a.FailureHeader, failureHeader);

        /// <summary>
        /// Sets the description text shown when authentication fails.
        /// </summary>
        public AuthService1 SetFailureDescription(string failureDescription) => SetPropertyValue(a => a.FailureDescription, failureDescription);

        /// <summary>
        /// Adds a related service (token or logout service).
        /// </summary>
        public AuthService1 AddService(AuthService1 service) => SetPropertyValue(a => a.services, a => a.Services, services.Attach(service));

        /// <summary>
        /// Removes a related service.
        /// </summary>
        public AuthService1 RemoveService(AuthService1 service) => SetPropertyValue(a => a.services, a => a.Services, services.Detach(service));
    }
}