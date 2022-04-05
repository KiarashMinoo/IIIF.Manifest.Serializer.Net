using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Nodes
{
    [JsonConverter(typeof(ManifestJsonConverter))]
    public class Manifest : BaseNode<Manifest>, IViewingDirectionSupport<Manifest>
    {
        public const string NavDateJName = "navDate";
        public const string SequencesJName = "sequences";
        public const string StructuresJName = "structures";

        private readonly List<Sequence> sequences = new List<Sequence>();
        private readonly List<Structure> structures = new List<Structure>();


        [JsonProperty(NavDateJName)]
        public DateTime? NavDate { get; private set; }

        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection ViewingDirection { get; private set; }

        [JsonProperty(SequencesJName)]
        public IReadOnlyCollection<Sequence> Sequences => sequences.AsReadOnly();

        [JsonProperty(StructuresJName)]
        public IReadOnlyCollection<Structure> Structures => structures.AsReadOnly();

        internal Manifest(string id) : base(id, "sc:Manifest")
        {
        }
        public Manifest(string id, IEnumerable<Label> labels) : this(id) => labels.Enumerate(label => AddLabel(label));
        public Manifest(string id, Label label) : this(id) => AddLabel(label);

        public Manifest SetNavDate(DateTime navDate) => SetPropertyValue(a => a.NavDate, navDate);
        public Manifest SetViewingDirection(ViewingDirection viewingDirection) => SetPropertyValue(a => a.ViewingDirection, viewingDirection);

        public Manifest AddSequence(Sequence sequence) => SetPropertyValue(a => a.sequences, a => a.Sequences, sequences.Attach(sequence));
        public Manifest RemoveSequence(Sequence sequence) => SetPropertyValue(a => a.sequences, a => a.Sequences, sequences.Detach(sequence));
    }
}