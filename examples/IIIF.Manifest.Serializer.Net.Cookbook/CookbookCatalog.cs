using System.Linq;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Net.Cookbook;

public sealed record ExampleDefinition(string Title, Func<object> Build)
{
    public void Run()
    {
        Console.WriteLine(Title);
        Console.WriteLine(new string('=', Title.Length));
        Console.WriteLine(JsonConvert.SerializeObject(Build(), Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
    }
}

/// <summary>
/// Faithful C# reconstructions of every real recipe in github.com/IIIF/cookbook-recipes (71
/// recipes; 0000_template/0231-transcript-meta-recipe/0466-link-for-loading-manifest are excluded
/// as non-recipes with no manifest JSON of their own). See SDK_VERSIONING_GUIDE.md for the
/// milestone history of the SDK features (Groups A-I) this catalog exercises.
///
/// This class is a thin Registry: it just aggregates a fixed list of <see cref="IRecipeSet"/>
/// Strategy implementations, each owning one thematically related slice of the recipes (see the
/// sibling <c>*Recipes.cs</c> files). Recipe construction logic itself lives in those classes, not
/// here, and shared construction helpers live in <see cref="RecipeBuilders"/>.
/// </summary>
public static class CookbookCatalog
{
    private static readonly IReadOnlyList<IRecipeSet> RecipeSets =
    [
        new FoundationRecipes(),
        new CanvasAndStructureRecipes(),
        new CollectionAndChoiceRecipes(),
        new MediaVariationRecipes(),
        new LinkingAndOperaRecipes(),
        new DescriptivePropertiesRecipes(),
        new ProviderAndTaggingRecipes(),
        new AnnotationCollectionRecipes(),
        new AdvancedCompositionRecipes()
    ];

    public static IReadOnlyList<ExampleDefinition> GetAll() =>
        RecipeSets.SelectMany(set => set.GetRecipes()).ToList();
}
