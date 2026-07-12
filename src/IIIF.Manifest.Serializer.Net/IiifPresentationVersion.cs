namespace IIIF.Manifests.Serializer;

/// <summary>
///     IIIF Presentation-family API versions this SDK detects and/or writes. See
///     <see cref="IiifPresentationVersionDetector" /> for how each value is recognized, and
///     <see cref="IiifSerializer" />'s per-resource `Serialize` overloads for which values can actually
///     be written (detecting a version and being able to write it are different things - see the
///     remarks on <see cref="Metadata_1_0" /> and <see cref="V4_0_Rc" />).
/// </summary>
public enum IiifPresentationVersion
{
    /// <summary>
    ///     No version could be determined: no recognized `@context`, and no 2.x/3.0 structural signal
    ///     at all (e.g. an empty object, or a document with only unrelated properties).
    /// </summary>
    Unknown = 0,

    /// <summary>
    ///     IIIF Metadata API 1.0 (`@context`: `http://www.shared-canvas.org/ns/context.json`) - the
    ///     "Shared Canvas" data model that predates and was superseded by Presentation API 2.0.
    ///     Structurally near-identical to Presentation 2.x (`sc:Manifest`, `sequences`/`canvases`,
    ///     `@id`/`@type`), so it is only reliably distinguished by this `@context` value - a Metadata
    ///     1.0 document with no `@context` detects as <see cref="V2_1" /> instead. Detected for
    ///     classification purposes only: this SDK does not import/export it as a first-class model.
    /// </summary>
    Metadata_1_0,

    /// <summary>
    ///     Presentation API 2.0. Shares an identical `@context`
    ///     (`http://iiif.io/api/presentation/2/context.json`) with 2.1 - confirmed against the live
    ///     spec, which documents no version field or structural difference between the two - so a
    ///     real-world 2.0 document is detected as <see cref="V2_1" />, never this value. This value is
    ///     reachable only via an explicit write request, or a non-standard
    ///     `.../presentation/2.0/context.json` some non-conformant tooling might emit.
    /// </summary>
    V2_0,

    /// <summary>
    ///     Presentation API 2.1/2.1.1 - the legacy output shape this SDK targets by default for any
    ///     detected 2.x (or Metadata 1.0-without-context) document.
    /// </summary>
    V2_1,

    /// <summary>
    ///     Presentation API 3.0 - the current stable spec and this SDK's default read/write target.
    /// </summary>
    V3_0,

    /// <summary>
    ///     Presentation API 4.0 (`@context`: `http://iiif.io/api/presentation/4/context.json`), still a
    ///     draft/release-candidate as of this writing (spec header: "4.0.0-draft"). Detected/tracked
    ///     for future-readiness only - never a default write target, and not currently a supported
    ///     write target at all (see <see cref="IiifSerializer" />'s version-dispatch `default`
    ///     branches, which throw <see cref="NotSupportedException" /> for this value).
    /// </summary>
    V4_0_Rc
}