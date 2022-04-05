using System;

namespace IIIF.Manifests.Serializer.Shared
{
    public class JsonNodeRequiredException<T> : Exception
    {
        public JsonNodeRequiredException(string jName) : base($"Invalid manifest json file, {jName} of {typeof(T)} is required")
        {
        }
    }
}