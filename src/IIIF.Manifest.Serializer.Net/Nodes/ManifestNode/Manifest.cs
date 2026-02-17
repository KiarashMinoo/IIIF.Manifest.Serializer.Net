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

        private readonly List<Sequence> sequences = new List<Sequence>();
        private readonly List<Structure> structures = new List<Structure>();


        [PresentationAPI("2.0")]
        [JsonProperty(NavDateJName)]
        public DateTime? NavDate { get; private set; }

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection ViewingDirection { get; private set; }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(SequencesJName)]
        public IReadOnlyCollection<Sequence> Sequences => sequences.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(StructuresJName)]
        public IReadOnlyCollection<Structure> Structures => structures.AsReadOnly();

        [PresentationAPI("2.0")]
        [JsonProperty(StartJName)]
        public string Start { get; private set; }

        [PresentationAPI("2.0")]
        [JsonProperty(PlaceholderCanvasJName)]
        public string PlaceholderCanvas { get; private set; }

        internal Manifest(string id) : base(id, "sc:Manifest")
        {
        }
        public Manifest(string id, IEnumerable<Label> labels) : this(id) => labels.Enumerate(label => AddLabel(label));
        public Manifest(string id, Label label) : this(id) => AddLabel(label);

        public Manifest SetNavDate(DateTime navDate) => SetPropertyValue(a => a.NavDate, navDate);
        public Manifest SetViewingDirection(ViewingDirection viewingDirection) => SetPropertyValue(a => a.ViewingDirection, viewingDirection);
        public Manifest SetStart(string start) => SetPropertyValue(a => a.Start, start);
        public Manifest SetPlaceholderCanvas(string placeholderCanvas) => SetPropertyValue(a => a.PlaceholderCanvas, placeholderCanvas);

        public Manifest AddSequence(Sequence sequence) => SetPropertyValue(a => a.sequences, a => a.Sequences, sequences.Attach(sequence));
        public Manifest RemoveSequence(Sequence sequence) => SetPropertyValue(a => a.sequences, a => a.Sequences, sequences.Detach(sequence));

        public Manifest AddStructure(Structure structure) => SetPropertyValue(a => a.structures, a => a.Structures, structures.Attach(structure));
        public Manifest RemoveStructure(Structure structure) => SetPropertyValue(a => a.structures, a => a.Structures, structures.Detach(structure));
    }
}