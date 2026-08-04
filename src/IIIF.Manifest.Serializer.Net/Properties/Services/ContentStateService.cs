using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services;

/// <summary>
///     IIIF Content State API 1.0 Service - provides compact representation of resource state.
/// </summary>
[ContentStateAPI("1.0", Notes = "Content State API 1.0 service for deep linking and state representation.")]
public sealed class ContentStateService : BaseItem<ContentStateService>, IBaseService
{
    /// <summary>
    ///     Parameterless constructor for materialization by EF Core (or other reflection-based ORMs) - not for
    ///     application code, which should always go through the other constructor overloads.
    /// </summary>
    private ContentStateService()
    {
    }

    [JsonConstructor]
    private ContentStateService(string id, string profile) : base(id, "ContentStateService")
    {
        Profile = profile;
    }

    /// <summary>
    ///     Creates a new ContentStateService.
    /// </summary>
    /// <param name="context">The IIIF context URL</param>
    /// <param name="id">The service identifier</param>
    /// <param name="profile">The service profile</param>
    public ContentStateService(string context, string id, string profile) : base(id, "ContentStateService", context)
    {
        Profile = profile;
    }

    [ContentStateAPI("1.0")]
    [JsonProperty(IBaseService.ProfileJName)]
    public string Profile
    {
        get => GetElementValue(x => x.Profile)!;
        private set => SetElementValue(value);
    }
}