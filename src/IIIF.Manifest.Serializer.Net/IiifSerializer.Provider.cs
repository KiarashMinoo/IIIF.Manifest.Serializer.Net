using System.Linq;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Per spec, <c>provider</c> is only valid on Manifest and Collection (unlike the other
/// BaseNode-generic extras in <c>IiifSerializer.NodeExtras.cs</c>), so it's written/read by its own
/// explicit call sites rather than the shared Canvas/Range-inclusive helper.
/// </summary>
public static partial class IiifSerializer
{
    private static void WriteV3Provider<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
    {
        var providers = node.Provider.Select(WriteV3ProviderEntry).ToList();
        if (providers.Count > 0)
        {
            obj["provider"] = new JArray(providers);
        }
    }

    private static void ReadV3Provider<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
    {
        foreach (var providerObj in obj["provider"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddProvider(ReadV3ProviderEntry(providerObj));
        }
    }

    private static JObject WriteV3ProviderEntry(Provider provider)
    {
        var obj = new JObject { ["id"] = provider.Id, ["type"] = "Agent" };
        obj["label"] = BuildLabelLanguageMapToken(provider.Label);

        var homepages = provider.Homepage.Select(WriteV3Homepage).ToList();
        if (homepages.Count > 0)
        {
            obj["homepage"] = new JArray(homepages);
        }

        var logos = provider.Logo.Select(WriteV3Logo).ToList();
        if (logos.Count > 0)
        {
            obj["logo"] = new JArray(logos);
        }

        var seeAlsos = provider.SeeAlso.Select(WriteV3SeeAlso).ToList();
        if (seeAlsos.Count > 0)
        {
            obj["seeAlso"] = new JArray(seeAlsos);
        }

        return obj;
    }

    private static Provider ReadV3ProviderEntry(JObject obj)
    {
        var provider = new Provider(ReadRequiredString(obj, "id"));
        var labels = ReadLabels(obj["label"]);
        if (labels.Count > 0)
        {
            provider.SetLabel(labels);
        }

        foreach (var homepageObj in obj["homepage"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            provider.AddHomepage(ReadV3Homepage(homepageObj));
        }

        foreach (var logoObj in obj["logo"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            provider.AddLogo(ReadV3Logo(logoObj));
        }

        foreach (var seeAlsoObj in obj["seeAlso"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            provider.AddSeeAlso(ReadV3SeeAlso(seeAlsoObj));
        }

        return provider;
    }
}
