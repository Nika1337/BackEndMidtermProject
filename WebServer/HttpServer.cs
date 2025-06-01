using System.Net;
using System.Net.Sockets;

namespace WebServer;

/// <summary>
/// Represents a basic TCP-based HTTP server that handles static file requests.
/// </summary>
public class HttpServer
{
    private readonly TcpListener _listener;
    private readonly RequestHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpServer"/> class.
    /// </summary>
    /// <param name="port">The port number to listen on.</param>
    /// <param name="rootDirectory">The root directory containing static files.</param>
    /// <param name="logFilePath">The file path to which request logs will be written.</param>
    /// <param name="allowedExtensions">The array of allowed file extensions (e.g., .html, .css, .js).</param>
    public HttpServer(int port, string rootDirectory, string logFilePath, string[] allowedExtensions)
    {
        _listener = new TcpListener(IPAddress.Any, port);
        _handler = new RequestHandler(rootDirectory, logFilePath, allowedExtensions);
    }

    /// <summary>
    /// Starts the HTTP server asynchronously and begins accepting client connections.
    /// </summary>
    /// <returns>A task representing the asynchronous server loop.</returns>
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
