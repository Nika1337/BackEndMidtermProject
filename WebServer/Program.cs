using System.Net;
using System.Net.Sockets;
using System.Text;

var portNumber = 8080;
var listener = new TcpListener(IPAddress.Any, portNumber);
listener.Start();
Console.WriteLine($"Listening for request started on port {portNumber}");

while (true)
{
    var client = await listener.AcceptTcpClientAsync();

    _ = Task.Run(() => HandleClient(client));
}

void HandleClient(TcpClient client)
{
    using var stream = client.GetStream();
    using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
    using var writer = new StreamWriter(stream) { AutoFlush = true };

    var requestLine = reader.ReadLine();
    if (requestLine == null || !requestLine.StartsWith("GET "))
    {
        client.Close();
        return;
    }

    string[] allowedExtensions = [".html", ".css", ".js"];

    string path = requestLine.Split(' ')[1];
    string ext = Path.GetExtension(path);

    if (!allowedExtensions.Contains(ext))
    {
        client.Close();
        return;
    }

    string filePath = Path.Combine("wwwroot", path.TrimStart('/'));

    if (File.Exists(filePath))
    {
        var content = File.ReadAllBytes(filePath);
        var mime = GetMimeType(ext);

        writer.WriteLine("HTTP/1.1 200 OK");
        writer.WriteLine($"Content-Type: {mime}");
        writer.WriteLine($"Content-Length: {content.Length}");
        writer.WriteLine();
        writer.Flush();
        stream.Write(content, 0, content.Length);
    }
    else
    {
        return;
    }


    client.Close();
}


string GetMimeType(string ext) => ext switch
{
    ".html" => "text/html",
    ".css" => "text/css",
    ".js" => "application/javascript",
    _ => "text/plain"
};
