using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content
{
    [JsonConverter(typeof(BaseContentJsonConverter<>))]
    public class BaseContent<TBaseContent> : BaseNode<TBaseContent>, IDimenssionSupport<TBaseContent>
        where TBaseContent : BaseContent<TBaseContent>
    {
        public const string FormatJName = "format";


        [JsonProperty(FormatJName)]
        public string Format { get; private set; }

        [JsonProperty(Constants.HeightJName)]
        public int? Height { get; private set; }

        [JsonProperty(Constants.WidthJName)]
        public int? Width { get; private set; }

        protected internal BaseContent(string id) : base(id)
        {
        }
        public BaseContent(string id, string type) : base(id, type)
        {
        }

        public TBaseContent SetFormat(string format) => SetPropertyValue(a => a.Format, format);

        public TBaseContent SetHeight(int height) => SetPropertyValue(a => a.Height, height);
        public TBaseContent SetWidth(int width) => SetPropertyValue(a => a.Width, width);
    }

    [JsonConverter(typeof(BaseContentJsonConverter<,>))]
    public class BaseContent<TContent, TResource> : BaseContent<TContent>
        where TContent : BaseContent<TContent, TResource>
        where TResource : BaseResource<TResource>
    {
        public const string ResourceJName = "resource";

        [JsonProperty(ResourceJName)]
        public TResource Resource { get; }

        public BaseContent(string id, string type, TResource resource) : base(id, type)
        {
            Resource = resource;
        }
    }
}