using WebServer;

await new HttpServer(
        port: 8080,
        rootDirectory: Path.Combine(AppContext.BaseDirectory, "wwwroot"),
        logFilePath: Path.Combine(AppContext.BaseDirectory, "log.txt"),
        allowedExtensions: [".html", ".css", ".js"]
    )
    .StartAsync();