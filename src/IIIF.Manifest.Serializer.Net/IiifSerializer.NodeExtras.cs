using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
///     Every BaseNode-generic descriptive/linking property valid on Manifest, Collection, Canvas, and
///     Range alike: rights, requiredStatement, partOf, summary, metadata, thumbnail, rendering,
///     homepage, seeAlso, behavior. <see cref="Properties.Provider" /> is wired up separately by its own
///     callers (see <c>IiifSerializer.Provider.cs</c>), since per spec it's only valid on
///     Manifest/Collection. The individual resource types read/written here each have their own
///     partial file: <c>IiifSerializer.Metadata.cs</c>, <c>IiifSerializer.ImageLikeResources.cs</c>
///     (thumbnail/logo), <c>IiifSerializer.LinkResources.cs</c> (rendering/homepage/seeAlso).
/// </summary>
public static partial class IiifSerializer
{
    private static void WriteV3NodeExtras<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
    {
        if (node.Rights is not null) obj["rights"] = node.Rights.Value;

        if (node.RequiredStatement is not null)
            obj["requiredStatement"] = new JObject
            {
                ["label"] = BuildLanguageMapToken(node.RequiredStatement.Label.Select(x => x.Value)),
                ["value"] = BuildLanguageMapToken(node.RequiredStatement.Value.Select(x => x.Value))
            };

        var partOf = node.PartOf.Select(x => new JObject { ["id"] = x.Id, ["type"] = x.Type }).ToList();
        if (partOf.Count > 0) obj["partOf"] = new JArray(partOf);

        if (node.Summary.Count > 0) obj["summary"] = BuildDescriptionLanguageMapToken(node.Summary);

        var metadata = node.Metadata.Select(WriteV3Metadata).ToList();
        if (metadata.Count > 0) obj["metadata"] = new JArray(metadata);

        if (node.Thumbnail is not null) obj["thumbnail"] = new JArray(WriteV3Thumbnail(node.Thumbnail));

        var rendering = node.Rendering.Select(WriteV3Rendering).ToList();
        if (rendering.Count > 0) obj["rendering"] = new JArray(rendering);

        var homepage = node.Homepage.Select(WriteV3Homepage).ToList();
        if (homepage.Count > 0) obj["homepage"] = new JArray(homepage);

        var seeAlso = node.SeeAlso.Select(WriteV3SeeAlso).ToList();
        if (seeAlso.Count > 0) obj["seeAlso"] = new JArray(seeAlso);

        // Embedded/inline services directly on this resource (BaseItem.Service) - distinct from
        // Manifest's top-level, 3.0-only centralized Services array (written separately in
        // WriteV3Manifest). Context-preserving (WriteV3Service, not WriteV3EmbeddedResourceService)
        // since an inline service on a real top-level resource legitimately declares its own
        // @context, unlike a service embedded on a content-resource body.
        var services = node.Service.Select(WriteV3Service).ToList();
        if (services.Count > 0) obj["service"] = new JArray(services);

        WriteV3AdditionalProperties(node, obj);
    }

    private static void ReadV3NodeExtras<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
    {
        if ((string?)obj["rights"] is { } rights) node.SetRights(new Rights(rights));

        if (obj["requiredStatement"] is JObject requiredStatementObj)
        {
            var label = ReadLabels(requiredStatementObj["label"]);
            var value = ReadLabels(requiredStatementObj["value"]).Select(x => new Description(x.Value)).ToList();
            node.SetRequiredStatement(new RequiredStatement(label, value));
        }

        foreach (var partOfObj in obj["partOf"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) node.AddPartOf(new PartOf(ReadRequiredString(partOfObj, "id"), (string?)partOfObj["type"] ?? "Manifest"));

        if (obj["summary"] is { } summaryToken) node.SetSummary(ReadDescriptions(summaryToken));

        foreach (var metadataObj in obj["metadata"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) node.AddMetadata(ReadV3Metadata(metadataObj));

        if (obj["thumbnail"]?.OfType<JObject>().FirstOrDefault() is { } thumbnailObj) node.SetThumbnail(ReadV3Thumbnail(thumbnailObj));

        foreach (var renderingObj in obj["rendering"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) node.AddRendering(ReadV3Rendering(renderingObj));

        foreach (var homepageObj in obj["homepage"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) node.AddHomepage(ReadV3Homepage(homepageObj));

        foreach (var seeAlsoObj in obj["seeAlso"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) node.AddSeeAlso(ReadV3SeeAlso(seeAlsoObj));

        foreach (var serviceObj in obj["service"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
            if (ReadV3Service(serviceObj) is { } service)
                node.AddService(service);

        // navPlace (extensions/IIIF.Manifest.Serializer.Net.NavPlace) - core can't reference the
        // extension assembly, so the JSON key name is necessarily hardcoded here; see
        // WriteV3AdditionalProperties/ReadV3AdditionalProperty's doc comments for why this is
        // targeted rather than a generic unknown-property sweep.
        ReadV3AdditionalProperty(obj, node, "navPlace");
    }
    

    private static void WriteV3Behavior<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
    {
        var behaviorValues = node.Behavior.Select(x => x.Value).ToList();
#pragma warning disable CS0618
        if (behaviorValues.Count == 0 && node.ViewingHint is not null) behaviorValues.Add(node.ViewingHint.Value);
#pragma warning restore CS0618

        if (behaviorValues.Count > 0) obj["behavior"] = new JArray(behaviorValues);
    }

    private static void ReadV3Behavior<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
    {
        foreach (var behavior in ReadStringArray(obj["behavior"])) node.AddBehavior(new Behavior(behavior));
    }
}