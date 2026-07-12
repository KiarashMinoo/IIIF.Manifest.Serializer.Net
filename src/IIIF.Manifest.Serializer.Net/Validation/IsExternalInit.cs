namespace System.Runtime.CompilerServices;

/// <summary>
///     Polyfill for the C# 9 <c>record</c>/<c>init</c> feature, which requires this marker type to
///     exist somewhere in the compilation. netstandard2.1 (this project's target) doesn't ship it
///     in the BCL - this is the standard, well-known workaround (identical to what
///     Microsoft.Bcl.HashCode-era or PolySharp-style polyfills provide), not a real runtime type.
/// </summary>
internal static class IsExternalInit;
