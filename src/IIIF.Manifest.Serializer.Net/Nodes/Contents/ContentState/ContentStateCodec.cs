using System.Text;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.ContentState;

/// <summary>
///     Encodes/decodes a <see cref="ContentState" /> to and from the base64url "content state string"
///     IIIF Content State API 1.0 uses for the <c>iiif-content</c> query parameter / URI fragment -
///     unpadded base64 of the UTF-8 minified JSON, with <c>+</c>/<c>/</c> replaced by <c>-</c>/<c>_</c>.
/// </summary>
public static class ContentStateCodec
{
    public static string Encode(ContentState contentState)
    {
        if (contentState is null) throw new ArgumentNullException(nameof(contentState));

        var json = JsonConvert.SerializeObject(contentState, Formatting.None, TrackableObject.JsonSerializerSettings);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public static ContentState Decode(string encoded)
    {
        if (string.IsNullOrWhiteSpace(encoded)) throw new ArgumentException("Content state string cannot be null or whitespace.", nameof(encoded));

        var base64 = encoded.Replace('-', '+').Replace('_', '/');
        var padding = base64.Length % 4;
        if (padding is 2 or 3)
            base64 = base64.PadRight(base64.Length + (4 - padding), '=');
        else if (padding == 1) throw new FormatException("Invalid content state string: incorrect length.");

        var json = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        return JsonConvert.DeserializeObject<ContentState>(json, TrackableObject.JsonSerializerSettings)
               ?? throw new JsonSerializationException("Could not decode IIIF content state.");
    }
}