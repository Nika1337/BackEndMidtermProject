

using System.Text;

namespace WebServer;
public static class HttpResponseWriter
{
    public static void SendSuccess(StreamWriter writer, byte[] bodyBytes, string contentType)
    {
        writer.Write($"HTTP/1.1 200 OK\r\n");
        writer.Write($"Content-Type: {contentType}\r\n");
        writer.Write($"Content-Length: {bodyBytes.Length}\r\n");
        writer.Write($"\r\n"); // End of headers
        writer.Flush();

        // Write body as raw bytes
        writer.BaseStream.Write(bodyBytes, 0, bodyBytes.Length);
    }

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
        writer.Write("\r\n"); // End of headers
        writer.Write(body);
        writer.Flush();
    }
}
