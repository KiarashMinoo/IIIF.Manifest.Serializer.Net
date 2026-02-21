using System;
using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Nodes.SequenceNode;
using IIIF.Manifests.Serializer.Nodes.StructureNode;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ManifestNode
{
    /// <summary>
    /// IIIF Manifest resource - the top-level resource representing a single object.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Core resource in Presentation API 2.x. In 3.0, structure changed significantly.")]
    [JsonConverter(typeof(ManifestJsonConverter))]
    public class Manifest : BaseNode<Manifest>, IViewingDirectionSupport<Manifest>
    {
        public const string NavDateJName = "navDate";
        public const string SequencesJName = "sequences";
        public const string StructuresJName = "structures";
        public const string StartJName = "start";
        public const string PlaceholderCanvasJName = "placeholderCanvas";

        [PresentationAPI("2.0")]
        [JsonProperty(NavDateJName)]
        public DateTime? NavDate => GetElementValue(a => a.NavDate);

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection? ViewingDirection => GetElementValue(a => a.ViewingDirection);

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(SequencesJName)]
        public IReadOnlyCollection<Sequence> Sequences => GetElementValue(x => x.Sequences) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(StructuresJName)]
        public IReadOnlyCollection<Structure> Structures => GetElementValue(x => x.Structures) ?? [];

        [PresentationAPI("2.0")]
        [JsonProperty(StartJName)]
        public string? Start => GetElementValue(a => a.Start);

        [PresentationAPI("2.0")]
        [JsonProperty(PlaceholderCanvasJName)]
        public string? PlaceholderCanvas => GetElementValue(a => a.PlaceholderCanvas);

        internal Manifest(string id) : base(id, "sc:Manifest")
        {
        }

        public Manifest(string id, IEnumerable<Label> labels) : this(id) => labels.Enumerate(label => AddLabel(label));
        public Manifest(string id, Label label) : this(id) => AddLabel(label);

        public Manifest SetNavDate(DateTime navDate) => SetElementValue(a => a.NavDate, navDate);
        public Manifest SetViewingDirection(ViewingDirection viewingDirection) => SetElementValue(a => a.ViewingDirection, viewingDirection);
        public Manifest SetStart(string start) => SetElementValue(a => a.Start, start);
        public Manifest SetPlaceholderCanvas(string placeholderCanvas) => SetElementValue(a => a.PlaceholderCanvas, placeholderCanvas);

        public Manifest AddSequence(Sequence sequence) => SetElementValue(a => a.Sequences, collection => collection.With(sequence));
        public Manifest RemoveSequence(Sequence sequence) => SetElementValue(a => a.Sequences, collection => collection.Without(sequence));

        public Manifest AddStructure(Structure structure) => SetElementValue(a => a.Structures, collection => collection.Without(structure));
        public Manifest RemoveStructure(Structure structure) => SetElementValue(a => a.Structures, collection => collection.With(structure));
    }
}