﻿using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// IIIF Presentation API behavior values.
    /// Replaces viewingHint in API 3.0 with expanded capabilities.
    /// </summary>
    [PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x. Some values also valid in 2.x as viewingHint.")]
    [JsonConverter(typeof(ValuableItemJsonConverter<Behavior>))]
    public class Behavior : ValuableItem<Behavior>
    {
        public Behavior(string value) : base(value)
        {
        }

        // Layout behaviors
        public static Behavior Paged => new Behavior("paged");
        public static Behavior Continuous => new Behavior("continuous");
        public static Behavior Individuals => new Behavior("individuals");
        public static Behavior Unordered => new Behavior("unordered");

        // Canvas behaviors
        public static Behavior FacingPages => new Behavior("facing-pages");
        public static Behavior NonPaged => new Behavior("non-paged");

        // Range behaviors
        public static Behavior Sequence => new Behavior("sequence");
        public static Behavior ThumbnailNav => new Behavior("thumbnail-nav");
        public static Behavior NoNav => new Behavior("no-nav");

        // Temporal behaviors
        public static Behavior AutoAdvance => new Behavior("auto-advance");
        public static Behavior NoAutoAdvance => new Behavior("no-auto-advance");
        public static Behavior Repeat => new Behavior("repeat");
        public static Behavior NoRepeat => new Behavior("no-repeat");

        // Collection behaviors
        public static Behavior MultiPart => new Behavior("multi-part");
        public static Behavior Together => new Behavior("together");

        // Annotation behaviors
        public static Behavior Hidden => new Behavior("hidden");
    }
}

