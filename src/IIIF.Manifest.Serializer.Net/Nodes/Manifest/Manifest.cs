using System;
using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Manifest
{
    [JsonConverter(typeof(ManifestJsonConverter))]
    public class Manifest : BaseNode<Manifest>, IViewingDirectionSupport<Manifest>
    {
        public const string NavDateJName = "navDate";
        public const string SequencesJName = "sequences";
        public const string StructuresJName = "structures";

        private readonly List<Sequence.Sequence> sequences = new List<Sequence.Sequence>();
        private readonly List<Structure.Structure> structures = new List<Structure.Structure>();


        [JsonProperty(NavDateJName)]
        public DateTime? NavDate { get; private set; }

        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection ViewingDirection { get; private set; }

        [JsonProperty(SequencesJName)]
        public IReadOnlyCollection<Sequence.Sequence> Sequences => sequences.AsReadOnly();

        [JsonProperty(StructuresJName)]
        public IReadOnlyCollection<Structure.Structure> Structures => structures.AsReadOnly();

        internal Manifest(string id) : base(id, "sc:Manifest")
        {
        }
        public Manifest(string id, IEnumerable<Label> labels) : this(id) => labels.Enumerate(label => AddLabel(label));
        public Manifest(string id, Label label) : this(id) => AddLabel(label);

        public Manifest SetNavDate(DateTime navDate) => SetPropertyValue(a => a.NavDate, navDate);
        public Manifest SetViewingDirection(ViewingDirection viewingDirection) => SetPropertyValue(a => a.ViewingDirection, viewingDirection);

        public Manifest AddSequence(Sequence.Sequence sequence) => SetPropertyValue(a => a.sequences, a => a.Sequences, sequences.Attach(sequence));
        public Manifest RemoveSequence(Sequence.Sequence sequence) => SetPropertyValue(a => a.sequences, a => a.Sequences, sequences.Detach(sequence));

        public Manifest AddStructure(Structure.Structure structure) => SetPropertyValue(a => a.structures, a => a.Structures, structures.Attach(structure));
        public Manifest RemoveStructure(Structure.Structure structure) => SetPropertyValue(a => a.structures, a => a.Structures, structures.Detach(structure));
    }
}