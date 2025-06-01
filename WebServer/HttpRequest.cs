

namespace WebServer;
public class HttpRequest
{
    public string Method { get; }
    public string FileName { get; }
    public string FileExtension { get; }
    public bool IsValid { get; }

    private HttpRequest(string method, string fileName, string fileExtension, bool isValid)
    {
        Method = method;
        FileName = fileName;
        FileExtension = fileExtension;
        IsValid = isValid;
    }

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