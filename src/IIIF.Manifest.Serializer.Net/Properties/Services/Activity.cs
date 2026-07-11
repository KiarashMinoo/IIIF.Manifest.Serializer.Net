using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties.Services.Discovery;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Services;

/// <summary>
/// Represents an Activity Streams activity for change discovery.
/// </summary>
[DiscoveryAPI("1.0")]
public class Activity : TrackableObject<Activity>
{
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string ObjectJName = "object";
    public const string TargetJName = "target";
    public const string StartTimeJName = "startTime";
    public const string EndTimeJName = "endTime";
    public const string SummaryJName = "summary";
    public const string ActorJName = "actor";

    [JsonProperty(IdJName)]
    public string? Id
    {
        get => GetElementValue(x => x.Id);
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(ObjectJName)]
    public ActivityObject Object
    {
        get => GetElementValue(x => x.Object)!;
        private set => SetElementValue(value);
    }

    /// <summary>
    /// The destination of a "Move" activity - required for that type; <see cref="Object"/> then
    /// carries the source the resource was moved from.
    /// </summary>
    [DiscoveryAPI("1.0")]
    [JsonProperty(TargetJName)]
    public DiscoveryResourceReference? Target
    {
        get => GetElementValue(x => x.Target);
        private set => SetElementValue(value);
    }

    [DiscoveryAPI("1.0")]
    [JsonProperty(StartTimeJName)]
    public string? StartTime
    {
        get => GetElementValue(x => x.StartTime);
        private set => SetElementValue(value);
    }

    [JsonProperty(EndTimeJName)]
    public string EndTime
    {
        get => GetElementValue(x => x.EndTime)!;
        private set => SetElementValue(value);
    }

    [DiscoveryAPI("1.0")]
    [JsonProperty(SummaryJName)]
    public string? Summary
    {
        get => GetElementValue(x => x.Summary);
        private set => SetElementValue(value);
    }

    [DiscoveryAPI("1.0")]
    [JsonProperty(ActorJName)]
    public DiscoveryAgent? Actor
    {
        get => GetElementValue(x => x.Actor);
        private set => SetElementValue(value);
    }

    public Activity(string type, ActivityObject @object, string endTime)
    {
        Type = type;
        Object = @object;
        EndTime = endTime;
    }

    public Activity SetId(string id)
    {
        Id = id;
        return this;
    }

    /// <summary>
    /// Sets the destination for a "Move" activity - <see cref="Object"/> should carry the source.
    /// </summary>
    public Activity SetTarget(DiscoveryResourceReference target)
    {
        Target = target;
        return this;
    }

    public Activity SetStartTime(string startTime)
    {
        StartTime = startTime;
        return this;
    }

    public Activity SetSummary(string summary)
    {
        Summary = summary;
        return this;
    }

    public Activity SetActor(DiscoveryAgent actor)
    {
        Actor = actor;
        return this;
    }
}
