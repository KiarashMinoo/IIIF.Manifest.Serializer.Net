using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    /// <summary>
    /// IIIF Content Search API 2.0 AutoComplete Service - provides autocomplete functionality for search.
    /// </summary>
    [SearchAPI("2.0", Notes = "AutoComplete service for Content Search API 2.0.")]
    [JsonConverter(typeof(AutoCompleteServiceJsonConverter))]
    public class AutoCompleteService : BaseItem<AutoCompleteService>, IBaseService
    {
        [SearchAPI("2.0")]
        [JsonProperty(IBaseService.ProfileJName)]
        public string Profile => GetElementValue(x => x.Profile)!;

        /// <summary>
        /// Creates a new AutoCompleteService.
        /// </summary>
        /// <param name="context">The IIIF context URL</param>
        /// <param name="id">The service identifier</param>
        /// <param name="profile">The service profile</param>
        public AutoCompleteService(string context, string id, string profile) : base(id, "AutoCompleteService2", context)
        {
            SetElementValue(x => x.Profile, profile);
        }
    }
}