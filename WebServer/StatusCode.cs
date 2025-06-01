namespace WebServer;

/// <summary>
/// Represents standard HTTP status codes used in the web server.
/// </summary>
public enum StatusCode
{
    /// <summary>
    /// 200 OK – The request has succeeded.
    /// </summary>
    Ok = 200,

    /// <summary>
    /// 400 Bad Request – The server could not understand the request due to invalid syntax.
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// 403 Forbidden – The server understood the request but refuses to authorize it.
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// 404 Not Found – The server can not find the requested resource.
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// 405 Method Not Allowed – The request method is known by the server but has been disabled and cannot be used.
    /// </summary>
    MethodNotAllowed = 405
}
