using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Service;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ServiceProperty
{
    /// <summary>
    /// IIIF Change Discovery API 1.0 Service - provides change discovery functionality using Activity Streams.
    /// </summary>
    [DiscoveryAPI("1.0", Notes = "Change Discovery API 1.0 service for tracking manifest changes.")]
    [JsonConverter(typeof(DiscoveryServiceJsonConverter))]
    public class DiscoveryService : BaseItem<DiscoveryService>, IBaseService
    {
        public const string OrderedItemsJName = "orderedItems";

        private readonly List<Activity> activities = new List<Activity>();

        [DiscoveryAPI("1.0")]
        [JsonProperty(IBaseService.ProfileJName)]
        public string Profile { get; }

        /// <summary>
        /// Creates a new DiscoveryService.
        /// </summary>
        /// <param name="context">The IIIF context URL</param>
        /// <param name="id">The service identifier</param>
        /// <param name="profile">The service profile</param>
        public DiscoveryService(string context, string id, string profile) : base(id, "OrderedCollection", context)
        {
            Profile = profile;
        }

        /// <summary>
        /// Gets the ordered list of activities representing changes.
        /// </summary>
        [DiscoveryAPI("1.0")]
        [JsonProperty(OrderedItemsJName)]
        public IReadOnlyCollection<Activity> OrderedItems => activities;

        /// <summary>
        /// Adds an activity to the ordered items.
        /// </summary>
        /// <param name="activity">The activity to add</param>
        /// <returns>This DiscoveryService for fluent API</returns>
        public DiscoveryService AddActivity(Activity activity)
        {
            activities.Add(activity);
            return this;
        }

        /// <summary>
        /// Removes an activity from the ordered items.
        /// </summary>
        /// <param name="activity">The activity to remove</param>
        /// <returns>This DiscoveryService for fluent API</returns>
        public DiscoveryService RemoveActivity(Activity activity)
        {
            activities.Remove(activity);
            return this;
        }
    }
}