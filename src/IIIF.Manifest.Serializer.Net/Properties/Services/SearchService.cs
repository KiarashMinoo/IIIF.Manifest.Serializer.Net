using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services;

/// <summary>
///     IIIF Content Search API 2.0 Service - provides search functionality for IIIF resources.
/// </summary>
[SearchAPI("2.0", Notes = "Content Search API 2.0 service for searching within manifests.")]
public class SearchService : UnprefixedBaseItem<SearchService>, IBaseService
{
    public new const string ServiceJName = "service";

    [JsonConstructor]
    private SearchService(string id, string profile) : base(id, "SearchService2")
    {
        Profile = profile;
    }

    /// <summary>
    ///     Creates a new SearchService.
    /// </summary>
    /// <param name="context">The IIIF context URL</param>
    /// <param name="id">The service identifier</param>
    /// <param name="profile">The service profile</param>
    public SearchService(string context, string id, string profile) : base(id, "SearchService2", context)
    {
        Profile = profile;
    }

    /// <summary>
    ///     Gets the autocomplete services associated with this search service.
    /// </summary>
    [SearchAPI("2.0")]
    [JsonProperty(ServiceJName)]
    public IReadOnlyCollection<AutoCompleteService> Services
    {
        get => GetElementValue(x => x.Services) ?? [];
        private set => SetElementValue(value);
    }

    [SearchAPI("2.0")]
    [JsonProperty(IBaseService.ProfileJName)]
    public string Profile
    {
        get => GetElementValue(x => x.Profile)!;
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Adds an autocomplete service to this search service.
    /// </summary>
    /// <param name="service">The autocomplete service to add</param>
    /// <returns>This SearchService for fluent API</returns>
    public SearchService AddService(AutoCompleteService service)
    {
        Services = Services.With(service);
        return this;
    }

    /// <summary>
    ///     Removes an autocomplete service from this search service.
    /// </summary>
    /// <param name="service">The autocomplete service to remove</param>
    /// <returns>This SearchService for fluent API</returns>
    public SearchService RemoveService(AutoCompleteService service)
    {
        Services = Services.Without(service);
        return this;
    }
}