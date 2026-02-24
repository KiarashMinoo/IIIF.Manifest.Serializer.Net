using System;
using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    /// <summary>
    /// IIIF Manifest resource - the top-level resource representing a single object.
    /// </summary>
    [PresentationAPI("2.0", Notes = "Core resource in Presentation API 2.x. In 3.0, structure changed significantly.")]
    public class Manifest : BaseNode<Manifest>, IViewingDirectionSupport<Manifest>
    {
        public const string NavDateJName = "navDate";
        public const string SequencesJName = "sequences";
        public const string StructuresJName = "structures";
        public const string StartJName = "start";
        public const string PlaceholderCanvasJName = "placeholderCanvas";

        [PresentationAPI("2.0")]
        [JsonProperty(NavDateJName)]
        public DateTime? NavDate
        {
            get => GetElementValue(a => a.NavDate);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(Constants.ViewingDirectionJName)]
        public ViewingDirection? ViewingDirection
        {
            get => GetElementValue(a => a.ViewingDirection);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "items")]
        [JsonProperty(SequencesJName)]
        public IReadOnlyCollection<Sequence> Sequences
        {
            get => GetElementValue(x => x.Sequences) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(StructuresJName)]
        public IReadOnlyCollection<Structure> Structures
        {
            get => GetElementValue(x => x.Structures) ?? [];
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(StartJName)]
        public string? Start
        {
            get => GetElementValue(a => a.Start);
            private set => SetElementValue(value);
        }

        [PresentationAPI("2.0")]
        [JsonProperty(PlaceholderCanvasJName)]
        public string? PlaceholderCanvas
        {
            get => GetElementValue(a => a.PlaceholderCanvas);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        internal Manifest(string id) : base(id, "sc:Manifest")
        {
        }

        public Manifest(string id, IReadOnlyCollection<Label> labels) : this(id) => SetLabel(labels);
        public Manifest(string id, Label label) : this(id) => AddLabel(label);

        public Manifest SetNavDate(DateTime navDate)
        {
            NavDate = navDate;
            return this;
        }

        public Manifest SetViewingDirection(ViewingDirection viewingDirection)
        {
            ViewingDirection = viewingDirection;
            return this;
        }

        public Manifest SetStart(string start)
        {
            Start = start;
            return this;
        }

        public Manifest SetPlaceholderCanvas(string placeholderCanvas)
        {
            PlaceholderCanvas = placeholderCanvas;
            return this;
        }

        public Manifest AddSequence(Sequence sequence)
        {
            Sequences = Sequences.With(sequence);
            return this;
        }

        public Manifest RemoveSequence(Sequence sequence)
        {
            Sequences = Sequences.Without(sequence);
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
    }
}