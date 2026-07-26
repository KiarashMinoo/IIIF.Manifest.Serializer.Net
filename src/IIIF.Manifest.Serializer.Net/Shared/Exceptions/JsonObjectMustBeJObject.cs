namespace IIIF.Manifests.Serializer.Shared.Exceptions;

public sealed class JsonObjectMustBeJObject<T> : Exception
{
    public JsonObjectMustBeJObject(string jName) : base($"Invalid manifest json file, {jName} of {typeof(T)} must be object")
    {
    }
}