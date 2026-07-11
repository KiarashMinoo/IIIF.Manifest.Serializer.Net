using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Choice;

/// <summary>
/// The Web Annotation Model's "Choice" body type (<c>type:"Choice"</c>, <c>items:[...]</c>) -
/// presents several alternative resources for the same body (e.g. "Natural Light" vs "X-Ray" in
/// cookbook recipe 0033-choice, or several audio formats in 0434-choice-av). Each item is
/// dispatched polymorphically via <see cref="BaseResourceJsonConverter"/>. Uses a dedicated
/// <see cref="ChoiceJsonConverter"/> rather than <see cref="ObjectArrayJsonConverter"/> because
/// <c>items</c> must always serialize as an array, even with a single element - unlike properties
/// where a lone value legitimately collapses to a bare scalar.
/// </summary>
[PresentationAPI("3.0")]
[JsonConverter(typeof(ChoiceJsonConverter))]
public class Choice : TrackableObject<Choice>, IBaseResource
{
    public const string TypeJName = "type";
    public const string ItemsJName = "items";

    ResourceType? IBaseResource.Type => new(Type);

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "Choice";
        private set => SetElementValue(value);
    }

    [JsonProperty(ItemsJName)]
    public IReadOnlyCollection<IBaseResource> Items
    {
        get => GetElementValue(x => x.Items) ?? [];
        private set => SetElementValue(value);
    }

    public Choice(IReadOnlyCollection<IBaseResource> items)
    {
        Type = "Choice";
        Items = items;
    }

    public Choice AddItem(IBaseResource item)
    {
        Items = Items.With(item);
        return this;
    }
}
