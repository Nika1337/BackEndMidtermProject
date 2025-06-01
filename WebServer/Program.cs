using System.Net;
using System.Net.Sockets;
using System.Text;

int port = 8080;
string rootDirectory = Path.Combine(AppContext.BaseDirectory, "wwwroot");
string logFilePath = Path.Combine(AppContext.BaseDirectory, "log.txt");
string[] allowedExtensions = [".html", ".css", ".js"];

var listener = new TcpListener(IPAddress.Any, port);
listener.Start();
Console.WriteLine($"Server started on port {port}");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();
    _ = Task.Run(() => HandleClient(client));
}

void HandleClient(TcpClient client)
{
    using var stream = client.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
    using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true) { AutoFlush = true };

    try
    {
        string? requestLine = reader.ReadLine();
        if (requestLine == null)
            return;

        File.AppendAllText(logFilePath, $"{DateTime.Now}: {requestLine}{Environment.NewLine}");

        string[] parts = requestLine.Split(' ');
        if (parts.Length != 3)
        {
            SendError(writer, 400, "Bad Request", "not_allowed.html");
            return;
        }

        string method = parts[0];
        string urlPath = parts[1];
        string ext = Path.GetExtension(urlPath);

        if (method != "GET")
        {
            SendError(writer, 405, "Method Not Allowed", "not_allowed.html");
            return;
        }

        if (!allowedExtensions.Contains(ext))
        {
            SendError(writer, 403, "Forbidden", "forbidden.html");
            return;
        }

        string relativePath = urlPath.TrimStart('/');
        string fileName = Path.GetFileName(relativePath);

        if (fileName is "not_allowed.html" or "forbidden.html" or "not_found.html")
        {
            SendError(writer, 403, "Forbidden", "forbidden.html");
            return;
        }

        string fullPath = Path.GetFullPath(Path.Combine(rootDirectory, relativePath));

        if (!fullPath.StartsWith(rootDirectory))
        {
            SendError(writer, 403, "Forbidden", "forbidden.html");
            return;
        }

        if (!File.Exists(fullPath))
        {
            SendError(writer, 404, "Not Found", "not_found.html");
            return;
        }

        byte[] content = File.ReadAllBytes(fullPath);
        string mime = GetMimeType(ext);

        writer.Write($"HTTP/1.1 200 OK\r\n");
        writer.Write($"Content-Type: {mime}\r\n");
        writer.Write($"Content-Length: {content.Length}\r\n");
        writer.Write($"\r\n");
        writer.Flush();

        stream.Write(content, 0, content.Length);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
    finally
    {
        client.Close();
    }
}

void SendError(StreamWriter writer, int code, string title, string errorFile)
{
    string errorPath = Path.Combine(rootDirectory, errorFile);
    string body = File.Exists(errorPath)
        ? File.ReadAllText(errorPath)
        : $"<html><body><h1>Error {code}: {title}</h1></body></html>";

    writer.Write($"HTTP/1.1 {code} {title}\r\n");
    writer.Write("Content-Type: text/html\r\n");
    writer.Write($"Content-Length: {Encoding.UTF8.GetByteCount(body)}\r\n");
    writer.Write("\r\n");
    writer.Write(body);
    writer.Flush();
}

string GetMimeType(string ext) => ext switch
{
    ".html" => "text/html",
    ".css" => "text/css",
    ".js" => "application/javascript",
    _ => "application/octet-stream"
};
