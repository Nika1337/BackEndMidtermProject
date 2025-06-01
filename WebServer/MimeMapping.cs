namespace WebServer;

/// <summary>
/// Provides MIME type mapping based on file extensions.
/// </summary>
public static class MimeMapping
{
    /// <summary>
    /// Dictionary mapping file extensions to their corresponding MIME types.
    /// </summary>
    private static readonly Dictionary<string, string> _mappings = new(StringComparer.OrdinalIgnoreCase)
    {
        [".html"] = "text/html",
        [".css"] = "text/css",
        [".js"] = "application/javascript",
        [".json"] = "application/json",
        [".png"] = "image/png",
        [".jpg"] = "image/jpeg",
        [".gif"] = "image/gif",
        [".svg"] = "image/svg+xml"
    };

    /// <summary>
    /// Gets the MIME type associated with a given file extension.
    /// </summary>
    /// <param name="extension">The file extension (including the dot, e.g., ".html").</param>
    /// <returns>The corresponding MIME type if known; otherwise, "application/octet-stream".</returns>
    public static string GetMimeType(string extension)
        => _mappings.TryGetValue(extension, out var mime)
            ? mime
            : "application/octet-stream";
}
