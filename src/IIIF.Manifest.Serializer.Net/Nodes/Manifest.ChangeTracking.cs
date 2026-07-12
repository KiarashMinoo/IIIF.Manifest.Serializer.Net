using System.Linq;
using IIIF.Manifests.Serializer.ChangeTracking;

namespace IIIF.Manifests.Serializer.Nodes;

/// <summary>
///     Changed-only manifest/delta output (issue #23). See <c>docs/CHANGE_TRACKING.md</c> for the
///     full design and its limitations - in short: <see cref="GetChangedManifest" /> is a
///     best-effort reconstruction covering the properties/scenarios this feature's own examples
///     exercise (label, rights, requiredStatement, summary, and canvas items that were added or
///     internally modified), not an exhaustive copy of every possible Manifest property. Removed
///     items are deliberately never represented here (a valid IIIF Manifest has no way to express
///     "this used to be here, now it's gone") - use <see cref="GetChangeSet" />'s <c>Changes</c>
///     list instead, which records every change including removals.
/// </summary>
public partial class Manifest
{
    public Manifest GetChangedManifest()
    {
        var changes = GetChanges();
        var changedOnly = new Manifest(Id, Label);

        if (Rights is not null && HasTopLevelChange(changes, nameof(Rights))) changedOnly.SetRights(Rights);

        if (RequiredStatement is not null && HasTopLevelChange(changes, nameof(RequiredStatement))) changedOnly.SetRequiredStatement(RequiredStatement);

        if (Summary.Count > 0 && HasTopLevelChange(changes, nameof(Summary))) changedOnly.SetSummary(Summary);

        var originalCanvases = Items.OfType<Canvas>().ToList();
        var changedCanvasIndices = changes
            .Where(x => x.Path.StartsWith($"{nameof(Items)}[", StringComparison.Ordinal))
            .Select(x => ExtractLeadingIndex(x.Path))
            .Where(i => i >= 0 && i < originalCanvases.Count)
            .Distinct()
            .OrderBy(i => i);

        foreach (var index in changedCanvasIndices) changedOnly.AddItem(originalCanvases[index]);

        return changedOnly;
    }

    /// <summary>
    ///     The delta envelope: <see cref="IiifChangeSet.Changes" /> is the complete, precise record
    ///     of every change (including removals); <see cref="IiifChangeSet.ChangedManifest" /> is the
    ///     same best-effort partial-Manifest reconstruction <see cref="GetChangedManifest" />
    ///     returns, provided together so a caller doesn't have to call <c>GetChanges()</c>
    ///     twice.
    /// </summary>
    public IiifChangeSet GetChangeSet()
    {
        return new IiifChangeSet(Id, "Manifest", DateTimeOffset.UtcNow, GetChanges(), GetChangedManifest());
    }

    private static bool HasTopLevelChange(IReadOnlyCollection<IiifChangeEntry> changes, string propertyName)
    {
        return changes.Any(x => x.Path == propertyName);
    }

    private static int ExtractLeadingIndex(string path)
    {
        var openBracket = path.IndexOf('[');
        if (openBracket < 0) return -1;

        var closeBracket = path.IndexOf(']', openBracket + 1);
        if (closeBracket < 0) return -1;

        return int.TryParse(path.Substring(openBracket + 1, closeBracket - openBracket - 1), out var index) ? index : -1;
    }
}
