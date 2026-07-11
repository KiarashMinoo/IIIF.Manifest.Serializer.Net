namespace IIIF.Manifests.Serializer.Net.Cookbook;

/// <summary>
/// Strategy interface: each implementation owns one thematically related slice of the 71 IIIF
/// Cookbook recipes and knows how to build its own <see cref="ExampleDefinition"/> entries.
/// <see cref="CookbookCatalog"/> is the Registry that aggregates every registered set.
/// </summary>
internal interface IRecipeSet
{
    IEnumerable<ExampleDefinition> GetRecipes();
}
