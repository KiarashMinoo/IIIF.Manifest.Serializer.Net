using IIIF.Manifests.Serializer.Properties.Interfaces;
using IIIF.Manifests.Serializer.Shared.BaseNode;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Content
{
    [JsonConverter(typeof(BaseContentJsonConverter<>))]
    public class BaseContent<TBaseContent> : BaseNode<TBaseContent>, IDimensionSupport<TBaseContent>
        where TBaseContent : BaseContent<TBaseContent>
    {
        public const string FormatJName = "format";

        [JsonProperty(FormatJName)] public string Format => GetElementValue(x => x.Format) ?? string.Empty;

        [JsonProperty(Constants.HeightJName)] public int? Height => GetElementValue(x => x.Height);

        [JsonProperty(Constants.WidthJName)] public int? Width => GetElementValue(x => x.Width);

        protected internal BaseContent(string id) : base(id)
        {
        }

        public BaseContent(string id, string type) : base(id, type)
        {
        }

        public TBaseContent SetFormat(string format) => SetElementValue(a => a.Format, format);

        public TBaseContent SetHeight(int height) => SetElementValue(a => a.Height, height);
        public TBaseContent SetWidth(int width) => SetElementValue(a => a.Width, width);
    }

    [JsonConverter(typeof(BaseContentJsonConverter<,>))]
    public class BaseContent<TContent, TResource> : BaseContent<TContent>
        where TContent : BaseContent<TContent, TResource>
        where TResource : BaseResource<TResource>
    {
        public const string ResourceJName = "resource";

        [JsonProperty(ResourceJName)] public TResource Resource => GetElementValue(x => x.Resource)!;

        public BaseContent(string id, string type, TResource resource) : base(id, type)
        {
            SetElementValue(x => x.Resource, resource);
        }
    }
}