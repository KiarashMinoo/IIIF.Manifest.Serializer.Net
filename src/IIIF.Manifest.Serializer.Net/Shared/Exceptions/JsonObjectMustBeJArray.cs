namespace IIIF.Manifests.Serializer.Shared.Exceptions;

public sealed class JsonObjectMustBeJArray<T> : Exception
{
    public JsonObjectMustBeJArray(string jName) : base($"Invalid manifest json file, {jName} of {typeof(T)} must be array")
    {
    }
}