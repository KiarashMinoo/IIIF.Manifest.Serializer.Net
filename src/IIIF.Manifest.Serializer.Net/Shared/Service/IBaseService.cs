using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Shared.Service;

[JsonConverter(typeof(ServiceJsonConverter))]
public interface IBaseService
{
    public const string ProfileJName = "profile";

    string Profile { get; }
}