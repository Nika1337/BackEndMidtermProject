
using System.Net;
using System.Net.Sockets;

namespace WebServer;
public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly RequestHandler _handler;

    public HttpServer(int port, string rootDirectory, string logFilePath, string[] allowedExtensions)
    {
        _listener = new TcpListener(IPAddress.Any, port);

        _handler = new RequestHandler(rootDirectory, logFilePath, allowedExtensions);
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine($"Server listening on port {((IPEndPoint)_listener.LocalEndpoint).Port}");

        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            _ = Task.Run(() => _handler.HandleClient(client));
        }
    }
}