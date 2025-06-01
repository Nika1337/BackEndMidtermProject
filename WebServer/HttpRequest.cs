namespace WebServer;

/// <summary>
/// Represents a parsed HTTP request with relevant metadata such as method, file name, and extension.
/// </summary>
public class HttpRequest
{
    /// <summary>
    /// The HTTP method (e.g., GET, POST) extracted from the request.
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// The requested file name (e.g., index.html).
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// The file extension of the requested file (e.g., .html, .css).
    /// </summary>
    public string FileExtension { get; }

    /// <summary>
    /// Indicates whether the request line was successfully parsed and valid.
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpRequest"/> class.
    /// </summary>
    /// <param name="method">HTTP method (e.g., GET).</param>
    /// <param name="fileName">Requested file name.</param>
    /// <param name="fileExtension">File extension (e.g., .html).</param>
    /// <param name="isValid">Indicates whether the request is valid.</param>
    private HttpRequest(string method, string fileName, string fileExtension, bool isValid)
    {
        Method = method;
        FileName = fileName;
        FileExtension = fileExtension;
        IsValid = isValid;
    }

    /// <summary>
    /// Parses the raw HTTP request line into an <see cref="HttpRequest"/> instance.
    /// </summary>
    /// <param name="requestLine">The raw HTTP request line (e.g., "GET /index.html HTTP/1.1").</param>
    /// <returns>A parsed <see cref="HttpRequest"/> object.</returns>
    public static HttpRequest Parse(string requestLine)
    {
        var tokens = requestLine.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length != 3)
        {
            return new HttpRequest(string.Empty, string.Empty, string.Empty, false);
        }

        string method = tokens[0];
        string rawPath = tokens[1];
        string fileName = rawPath.TrimStart('/');
        if (string.IsNullOrEmpty(fileName))
        {
            fileName = "index.html";
        }

        string fileExtension = Path.GetExtension(fileName);
        return new HttpRequest(method, fileName, fileExtension, true);
    }
}
