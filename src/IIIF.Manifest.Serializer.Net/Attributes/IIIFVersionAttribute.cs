using System;

namespace IIIF.Manifests.Serializer.Attributes
{
    /// <summary>
    /// Specifies which IIIF API versions support this class, property, or method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false)]
    public class IIIFVersionAttribute : Attribute
    {
        /// <summary>
        /// The minimum IIIF API version that supports this feature.
        /// </summary>
        public string MinVersion { get; }

        /// <summary>
        /// The maximum IIIF API version that supports this feature (null = current/latest).
        /// </summary>
        public string MaxVersion { get; }

        /// <summary>
        /// Indicates if this feature is deprecated in newer versions.
        /// </summary>
        public bool IsDeprecated { get; set; }

        /// <summary>
        /// The version in which this feature was deprecated (if applicable).
        /// </summary>
        public string DeprecatedInVersion { get; set; }

        /// <summary>
        /// Suggested replacement for deprecated features.
        /// </summary>
        public string ReplacedBy { get; set; }

        /// <summary>
        /// Additional notes about version compatibility.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Creates an attribute indicating API version support.
        /// </summary>
        /// <param name="minVersion">Minimum supported version (e.g., "2.0")</param>
        /// <param name="maxVersion">Maximum supported version (null = still supported)</param>
        public IIIFVersionAttribute(string minVersion, string maxVersion = null)
        {
            MinVersion = minVersion;
            MaxVersion = maxVersion;
        }
    }

    /// <summary>
    /// Indicates this feature is part of IIIF Presentation API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class PresentationAPIAttribute : IIIFVersionAttribute
    {
        public PresentationAPIAttribute(string minVersion, string maxVersion = null) 
            : base(minVersion, maxVersion)
        {
        }
    }

    /// <summary>
    /// Indicates this feature is part of IIIF Image API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class ImageAPIAttribute : IIIFVersionAttribute
    {
        public ImageAPIAttribute(string minVersion, string maxVersion = null) 
            : base(minVersion, maxVersion)
        {
        }
    }

    /// <summary>
    /// Indicates this feature is part of IIIF Auth API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthAPIAttribute : IIIFVersionAttribute
    {
        public AuthAPIAttribute(string minVersion, string maxVersion = null) 
            : base(minVersion, maxVersion)
        {
        }
    }

    /// <summary>
    /// Indicates this feature is part of IIIF Search API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class SearchAPIAttribute : IIIFVersionAttribute
    {
        public SearchAPIAttribute(string minVersion, string maxVersion = null) 
            : base(minVersion, maxVersion)
        {
        }
    }

    /// <summary>
    /// Indicates this feature is part of IIIF Change Discovery API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class DiscoveryAPIAttribute : IIIFVersionAttribute
    {
        public DiscoveryAPIAttribute(string minVersion, string maxVersion = null) 
            : base(minVersion, maxVersion)
        {
        }
    }

    /// <summary>
    /// Indicates this feature is part of IIIF Content State API.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
    public class ContentStateAPIAttribute : IIIFVersionAttribute
    {
        public ContentStateAPIAttribute(string minVersion, string maxVersion = null) 
            : base(minVersion, maxVersion)
        {
        }
    }
}

