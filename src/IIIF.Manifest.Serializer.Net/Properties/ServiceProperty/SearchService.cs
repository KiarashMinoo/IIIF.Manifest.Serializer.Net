using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    /// <summary>
    /// IIIF Content Search API 2.0 Service - provides search functionality for IIIF resources.
    /// </summary>
    [SearchAPI("2.0", Notes = "Content Search API 2.0 service for searching within manifests.")]
    [JsonConverter(typeof(SearchServiceJsonConverter))]
    public class SearchService : BaseItem<SearchService>, IBaseService
    {
        public new const string ServiceJName = "service";

        private readonly List<AutoCompleteService> services = new List<AutoCompleteService>();

        [SearchAPI("2.0")]
        [JsonProperty(IBaseService.ProfileJName)]
        public string Profile { get; }

        /// <summary>
        /// Creates a new SearchService.
        /// </summary>
        /// <param name="id">The service identifier</param>
        /// <param name="profile">The service profile</param>
        public SearchService(string context, string id, string profile) : base(id, "SearchService2", context)
        {
            Profile = profile;
        }

        /// <summary>
        /// Gets the autocomplete services associated with this search service.
        /// </summary>
        [SearchAPI("2.0")]
        [JsonProperty(ServiceJName)]
        public IReadOnlyCollection<AutoCompleteService> Services => services;

        /// <summary>
        /// Adds an autocomplete service to this search service.
        /// </summary>
        /// <param name="service">The autocomplete service to add</param>
        /// <returns>This SearchService for fluent API</returns>
        public SearchService AddService(AutoCompleteService service)
        {
            services.Add(service);
            return this;
        }

        /// <summary>
        /// Removes an autocomplete service from this search service.
        /// </summary>
        /// <param name="service">The autocomplete service to remove</param>
        /// <returns>This SearchService for fluent API</returns>
        public SearchService RemoveService(AutoCompleteService service)
        {
            services.Remove(service);
            return this;
        }
    }
}