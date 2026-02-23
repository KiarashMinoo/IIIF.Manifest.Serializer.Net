using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content
{
    public class BaseContent<TBaseContent> : BaseNode<TBaseContent>, IDimensionSupport<TBaseContent>
        where TBaseContent : BaseContent<TBaseContent>
    {
        public const string FormatJName = "format";

        [JsonProperty(FormatJName)]
        public string Format
        {
            get => GetElementValue(x => x.Format) ?? string.Empty;
            private set => SetElementValue(value);
        }

        [JsonProperty(Constants.HeightJName)]
        public int? Height
        {
            get => GetElementValue(x => x.Height);
            private set => SetElementValue(value);
        }

        [JsonProperty(Constants.WidthJName)]
        public int? Width
        {
            get => GetElementValue(x => x.Width);
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        protected internal BaseContent(string id) : base(id)
        {
        }

        public BaseContent(string id, string type) : base(id, type)
        {
        }

        public TBaseContent SetFormat(string format)
        {
            Format = format;
            return (TBaseContent)this;
        }

        public TBaseContent SetHeight(int height)
        {
            Height = height;
            return (TBaseContent)this;
        }

        public TBaseContent SetWidth(int width)
        {
            Width = width;
            return (TBaseContent)this;
        }
    }

    public class BaseContent<TContent, TResource> : BaseContent<TContent>
        where TContent : BaseContent<TContent, TResource>
        where TResource : BaseResource<TResource>
    {
        public const string ResourceJName = "resource";

        [JsonProperty(ResourceJName)]
        public TResource Resource
        {
            get => GetElementValue(x => x.Resource)!;
            private set => SetElementValue(value);
        }

        [JsonConstructor]
        public BaseContent(string id, string type, TResource resource) : base(id, type)
        {
            Resource = resource;
        }
    }
}