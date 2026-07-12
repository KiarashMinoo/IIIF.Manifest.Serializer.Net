namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
///     Entry point for explicit Georeference extension bootstrap.
/// </summary>
public static class GeoreferenceExtensions
{
    private static int isRegistered;

    /// <summary>
    ///     Registers Georeference extension dependencies with core serializers.
    ///     Safe to call multiple times.
    /// </summary>
    public static void Register()
    {
        if (Interlocked.Exchange(ref isRegistered, 1) == 1) return;

        // Georeference depends on navPlace types and may deserialize embedded Feature bodies.
        NavPlaceExtensions.Register();
    }
}