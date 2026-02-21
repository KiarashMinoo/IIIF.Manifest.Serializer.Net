namespace IIIF.Manifests.Serializer.Shared.Service;

public interface IBaseService
{
    public const string ProfileJName = "profile";

    string Profile { get; }
}