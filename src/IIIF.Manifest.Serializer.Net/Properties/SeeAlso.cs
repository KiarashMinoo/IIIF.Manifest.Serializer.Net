using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [PresentationAPI("2.0")]
    public class SeeAlso : FormattableItem<SeeAlso>
    {
        public const string ProfileJName = "profile";
        public const string LabelJName = "label";

        [JsonProperty(ProfileJName)]
        public string? Profile
        {
            get => GetElementValue(x => x.Profile);
            private set => SetElementValue(value);
        }

        [JsonProperty(LabelJName)]
        public string? Label
        {
            get => GetElementValue(x => x.Label);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public SeeAlso(string id) : base(id)
        {
        }

        public SeeAlso SetProfile(string profile)
        {
            Profile = profile;
            return this;
        }

        public SeeAlso SetLabel(string label)
        {
            Label = label;
            return this;
        }

        /// <summary>
        /// Publicly exposes the (otherwise <c>internal</c>) resource type - <c>seeAlso</c> entries
        /// commonly carry a specific type like <c>Dataset</c> (cookbook recipes 0053/0068/0234),
        /// unlike Rendering/Homepage which the SDK defaults to <c>Text</c>.
        /// </summary>
        public new SeeAlso SetType(string type)
        {
            base.SetType(type);
            return this;
        }
    }
}
