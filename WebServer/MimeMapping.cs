

namespace WebServer;
public static class MimeMapping
{
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

    public static string GetMimeType(string extension)
        => _mappings.TryGetValue(extension, out var mime)
            ? mime
            : "application/octet-stream";
}