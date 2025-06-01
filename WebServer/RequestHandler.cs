using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WebServer;

/// <summary>
/// Handles HTTP requests from TCP clients.
/// </summary>
public class RequestHandler
{
    private readonly string _rootDirectory;
    private readonly HashSet<string> _allowedExtensions;
    private static readonly string[] _protectedFiles = { "not_allowed.html", "forbidden.html", "not_found.html" };
    private readonly Logger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestHandler"/> class.
    /// </summary>
    /// <param name="rootDirectory">Root directory where static files are served from.</param>
    /// <param name="logFilePath">Path to the log file for request logging.</param>
    /// <param name="allowedExtensions">Array of allowed file extensions.</param>
    public RequestHandler(string rootDirectory, string logFilePath, string[] allowedExtensions)
    {
        _rootDirectory = Path.GetFullPath(rootDirectory);
        _allowedExtensions = new HashSet<string>(allowedExtensions, StringComparer.OrdinalIgnoreCase);
        _logger = new Logger(logFilePath);
    }

    /// <summary>
    /// Handles a single TCP client connection and serves an HTTP response.
    /// </summary>
    /// <param name="client">The connected <see cref="TcpClient"/>.</param>
    public void HandleClient(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            using var writer = new StreamWriter(stream, new UTF8Encoding(false), leaveOpen: true)
            {
                AutoFlush = true
            };

            string? requestLine = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(requestLine))
                return;

            LogRequest(client, requestLine);

            var request = HttpRequest.Parse(requestLine);
            if (!request.IsValid)
            {
                HttpResponseWriter.SendError(writer, StatusCode.BadRequest, "Bad Request", "not_allowed.html", _rootDirectory);
                return;
            }

            if (!ValidateMethod(request.Method))
            {
                HttpResponseWriter.SendError(writer, StatusCode.MethodNotAllowed, "Method Not Allowed", "not_allowed.html", _rootDirectory);
                return;
            }

            var fileName = request.FileName;
            if (IsProtectedFile(fileName))
            {
                HttpResponseWriter.SendError(writer, StatusCode.Forbidden, "Forbidden", "forbidden.html", _rootDirectory);
                return;
            }

            if (!ValidateExtension(request.FileExtension))
            {
                HttpResponseWriter.SendError(writer, StatusCode.Forbidden, "Forbidden", "forbidden.html", _rootDirectory);
                return;
            }

            var fullPath = Path.GetFullPath(Path.Combine(_rootDirectory, fileName));
            if (!fullPath.StartsWith(_rootDirectory, StringComparison.Ordinal))
            {
                HttpResponseWriter.SendError(writer, StatusCode.Forbidden, "Forbidden", "forbidden.html", _rootDirectory);
                return;
            }

            if (!File.Exists(fullPath))
            {
                HttpResponseWriter.SendError(writer, StatusCode.NotFound, "Not Found", "not_found.html", _rootDirectory);
                return;
            }

            var content = File.ReadAllBytes(fullPath);
            var contentType = MimeMapping.GetMimeType(request.FileExtension);
            HttpResponseWriter.SendSuccess(writer, content, contentType);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    /// <summary>
    /// Validates if the HTTP method is supported.
    /// </summary>
    private static bool ValidateMethod(string method) =>
        string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if a file extension is allowed.
    /// </summary>
    private bool ValidateExtension(string ext) =>
        _allowedExtensions.Contains(ext);

    /// <summary>
    /// Checks whether the requested file is a protected internal error page.
    /// </summary>
    private static bool IsProtectedFile(string fileName) =>
        _protectedFiles.Contains(Path.GetFileName(fileName), StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Logs the incoming request to the logger with IP and timestamp.
    /// </summary>
    private void LogRequest(TcpClient client, string requestLine)
    {
        string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString() ?? "N/A";
        string timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logEntry = $"{timeStamp} - {clientIp} - {requestLine}";
        _logger.Log(logEntry);
    }
}
