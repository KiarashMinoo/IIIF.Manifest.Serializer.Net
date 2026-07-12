using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.SystemTextJson;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes;

/// <summary>
///     IIIF Manifest resource - the top-level resource representing a single object.
/// </summary>
[PresentationAPI("2.0", Notes = "Core resource in Presentation API 2.x. In 3.0, structure changed significantly.")]
[System.Text.Json.Serialization.JsonConverter(typeof(ManifestSystemTextJsonConverter))]
public partial class Manifest : BaseNode<Manifest>, IViewingDirectionSupport<Manifest>
{
    public const string NavDateJName = "navDate";
    public const string SequencesJName = "sequences";
    public const string StructuresJName = "structures";
    public const string StartJName = "start";
    public const string PlaceholderCanvasJName = "placeholderCanvas";
    public const string ServicesJName = "services";

    [Newtonsoft.Json.JsonConstructor]
    internal Manifest(string id) : base(id, "sc:Manifest")
    {
    }

    public Manifest(string id, IReadOnlyCollection<Label> labels) : this(id)
    {
        SetLabel(labels);
    }

    public Manifest(string id, Label label) : this(id)
    {
        AddLabel(label);
    }

    /// <summary>
    ///     Top-level centralized services, new in Presentation API 3.0. Lets a service
    ///     referenced by multiple resources be declared once here instead of inlined on every
    ///     resource via <see cref="BaseItem{TBaseItem}.Service" />. No 2.x equivalent — this SDK
    ///     always inlines services on write (see SDK_VERSIONING_GUIDE.md §5); this property only
    ///     supports reading (and round-tripping) documents that already centralize services here.
    /// </summary>
    [PresentationAPI("3.0", Notes = "New in 3.0. Centralizes services referenced by multiple resources; no 2.x equivalent.")]
    [JsonProperty(ServicesJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<IBaseService> Services
    {
        get => GetElementValue(x => x.Services) ?? [];
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(NavDateJName)]
    public DateTime? NavDate
    {
        get => GetElementValue(a => a.NavDate);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Legacy (2.x) view of this Manifest's canvas sequencing. Computed from
    ///     <see cref="BaseNode{TBaseNode}.Items" /> (the 3.0-native storage) plus
    ///     <see cref="ViewingDirection" />/<see cref="Start" />; not itself the backing store.
    ///     A 2.x document may contain more than one sequence — only the first becomes the
    ///     primary view backed by Items; any additional sequences are preserved verbatim on
    ///     <see cref="AdditionalSequences" /> rather than silently dropped.
    /// </summary>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [JsonProperty(SequencesJName)]
    public IReadOnlyCollection<Sequence> Sequences
    {
        get => BuildLegacySequences();
        private set => ReplaceFromLegacySequences(value ?? []);
    }

    /// <summary>
    ///     Legacy-only: sequences beyond the first on a 2.x document with multiple sequences.
    ///     IIIF 3.0 has no equivalent (Manifest.items models exactly one canvas ordering), so
    ///     these cannot be represented in 3.0-native storage; kept here purely so multi-sequence
    ///     legacy documents round-trip through legacy JSON without silent data loss.
    /// </summary>
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0",
        Notes = "No 3.0 equivalent. Only the first sequence's canvases become Items; the rest are preserved here, not dropped.")]
    [JsonIgnore]
    public IReadOnlyCollection<Sequence> AdditionalSequences
    {
        get => GetElementValue(x => x.AdditionalSequences) ?? [];
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Legacy-only, purely a JSON-shaping affordance: sets the identity used for the
    ///     synthesized <see cref="Sequence" /> when this Manifest is serialized as Presentation
    ///     API 2.x. Has no effect on 3.0 output (3.0 has no sequence concept), so this is not
    ///     tagged obsolete despite being legacy-oriented.
    /// </summary>
    [JsonIgnore]
    private string? PrimarySequenceId
    {
        get => GetElementValue<string?>();
        set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(StructuresJName)]
    public IReadOnlyCollection<Structure> Structures
    {
        get => GetElementValue(x => x.Structures) ?? [];
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Reuses <see cref="AnnotationTarget" /> since "start" allows the same shapes an
    ///     Annotation's target does (spec: a plain reference, or a SpecificResource+selector for
    ///     e.g. starting mid-recording at a given time - cookbook recipe 0015-start).
    /// </summary>
    [PresentationAPI("3.0", Notes = "3.0-only; 2.x used a Sequence's startCanvas instead (a plain Canvas reference).")]
    [JsonProperty(StartJName)]
    public AnnotationTarget? Start
    {
        get => GetElementValue(a => a.Start);
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     A full Canvas shown before this Manifest's own content is available/rendered. 3.0-only
    ///     - despite the pre-existing (and wrong) "2.0" tag this property previously carried, 2.x
    ///     has no placeholderCanvas concept at all. See also <see cref="Canvas.PlaceholderCanvas" />,
    ///     which is what cookbook recipe 0013-placeholderCanvas actually exercises (this
    ///     Manifest-level property has the identical shape per spec §5.4.2, just not covered by
    ///     a recipe).
    /// </summary>
    [PresentationAPI("3.0")]
    [JsonProperty(PlaceholderCanvasJName)]
    public Canvas? PlaceholderCanvas
    {
        get => GetElementValue(a => a.PlaceholderCanvas);
        private set => SetElementValue(value);
    }

    [PresentationAPI("2.0")]
    [JsonProperty(Constants.ViewingDirectionJName)]
    public ViewingDirection? ViewingDirection
    {
        get => GetElementValue(a => a.ViewingDirection);
        private set => SetElementValue(value);
    }

    public Manifest SetViewingDirection(ViewingDirection viewingDirection)
    {
        ViewingDirection = viewingDirection;
        return this;
    }

    public Manifest SetSequenceId(string id)
    {
        PrimarySequenceId = id;
        return this;
    }

    public Manifest SetNavDate(DateTime navDate)
    {
        NavDate = navDate;
        return this;
    }

    public Manifest SetStart(AnnotationTarget start)
    {
        Start = start;
        return this;
    }

    public Manifest SetPlaceholderCanvas(Canvas placeholderCanvas)
    {
        PlaceholderCanvas = placeholderCanvas;
        return this;
    }

    /// <summary>
    ///     Adds a service to the top-level, centralized <see cref="Services" /> array (3.0-only).
    ///     Distinct from <see cref="BaseItem{TBaseItem}.AddService{TService}" />, which inlines a
    ///     service directly on this resource via the <c>service</c> property.
    /// </summary>
    public Manifest AddTopLevelService<TService>(TService service) where TService : IBaseService
    {
        Services = Services.With(service);
        return this;
    }

    public Manifest RemoveTopLevelService<TService>(TService service) where TService : IBaseService
    {
        Services = Services.Without(service);
        return this;
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct canvases directly via AddItem instead.")]
    public Manifest AddSequence(Sequence sequence)
    {
        ReplaceFromLegacySequences(Sequences.With(sequence));
        return this;
    }

    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
    [Obsolete("Deprecated in IIIF Presentation API 3.0. Construct canvases directly via AddItem instead.")]
    public Manifest RemoveSequence(Sequence sequence)
    {
        ReplaceFromLegacySequences(Sequences.Without(sequence));
        return this;
    }

    public Manifest AddStructure(Structure structure)
    {
        Structures = Structures.With(structure);
        return this;
    }

    public Manifest RemoveStructure(Structure structure)
    {
        Structures = Structures.Without(structure);
        return this;
    }

    private IReadOnlyCollection<Sequence> BuildLegacySequences()
    {
        var canvases = Items.OfType<Canvas>().ToList();
        if (canvases.Count == 0 && PrimarySequenceId is null && AdditionalSequences.Count == 0) return [];

        var primary = new Sequence(PrimarySequenceId ?? $"{Id}/sequence/normal");
        foreach (var canvas in canvases) primary.AddCanvas(canvas);

        if (ViewingDirection is not null) primary.SetViewingDirection(ViewingDirection);

        if (Start is not null) primary.SetStartCanvas(new StartCanvas(Start.SourceId));

        return [primary, .. AdditionalSequences];
    }

    private void ReplaceFromLegacySequences(IReadOnlyCollection<Sequence> sequences)
    {
        foreach (var existingCanvas in Items.OfType<Canvas>().ToList()) RemoveItem(existingCanvas);

        var list = sequences.ToList();
        if (list.Count == 0)
        {
            PrimarySequenceId = null;
            AdditionalSequences = [];
            return;
        }

        var primary = list[0];
        PrimarySequenceId = primary.Id;

        foreach (var canvas in primary.Canvases) AddItem(canvas);

        if (primary.ViewingDirection is not null && ViewingDirection is null) SetViewingDirection(primary.ViewingDirection);

        if (primary.StartCanvas is not null && Start is null) SetStart(primary.StartCanvas.Id);

        AdditionalSequences = list.Skip(1).ToList();
    }
}