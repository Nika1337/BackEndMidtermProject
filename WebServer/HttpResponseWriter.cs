using System.Text;

namespace WebServer;

/// <summary>
/// Utility class for writing HTTP responses to the output stream.
/// </summary>
public static class HttpResponseWriter
{
    /// <summary>
    /// Sends a successful HTTP 200 OK response with the given content type and body.
    /// </summary>
    /// <param name="writer">The <see cref="StreamWriter"/> used to write headers.</param>
    /// <param name="bodyBytes">The raw content bytes to write to the response body.</param>
    /// <param name="contentType">The MIME type of the response content (e.g., text/html).</param>
    public static void SendSuccess(StreamWriter writer, byte[] bodyBytes, string contentType)
    {
        writer.Write($"HTTP/1.1 200 OK\r\n");
        writer.Write($"Content-Type: {contentType}\r\n");
        writer.Write($"Content-Length: {bodyBytes.Length}\r\n");
        writer.Write($"\r\n");
        writer.Flush();

        writer.BaseStream.Write(bodyBytes, 0, bodyBytes.Length);
    }

    /// <summary>
    /// Sends an HTTP error response with the given status code and optional error file.
    /// </summary>
    /// <param name="writer">The <see cref="StreamWriter"/> used to write headers and body.</param>
    /// <param name="statusCode">The HTTP status code to return (e.g., 404).</param>
    /// <param name="reasonPhrase">The textual description of the status code (e.g., "Not Found").</param>
    /// <param name="errorFileName">The filename of the custom error page to return if it exists.</param>
    /// <param name="rootDirectory">The root directory where static files are located.</param>
    public static void SendError(
        StreamWriter writer,
        StatusCode statusCode,
        string reasonPhrase,
        string errorFileName,
        string rootDirectory)
    {
        var errorPath = Path.Combine(rootDirectory, errorFileName);
        string body = File.Exists(errorPath)
            ? File.ReadAllText(errorPath)
            : $"<html><body><h1>Error {(int)statusCode}: {reasonPhrase}</h1></body></html>";

        writer.Write($"HTTP/1.1 {(int)statusCode} {reasonPhrase}\r\n");
        writer.Write("Content-Type: text/html\r\n");
        writer.Write($"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n");
        writer.Write("\r\n");
        writer.Write(body);
        writer.Flush();
    }
}
