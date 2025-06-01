using System.Collections.Concurrent;
using System.Text;

namespace WebServer;

/// <summary>
/// Asynchronous logger that writes log messages to a file using a background task and a thread-safe queue.
/// </summary>
public class Logger
{
    private readonly ConcurrentQueue<string> _logQueue = new();
    private readonly AutoResetEvent _logSignal = new(false);
    private readonly string _logFilePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class and starts the background log processor.
    /// </summary>
    /// <param name="logFilePath">The file path where logs will be written.</param>
    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        Task.Run(ProcessLogQueue);
    }

    /// <summary>
    /// Enqueues a log message to be written to the log file.
    /// </summary>
    /// <param name="message">The log message to be recorded.</param>
    public void Log(string message)
    {
        _logQueue.Enqueue($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        _logSignal.Set();
    }

    /// <summary>
    /// Continuously processes log entries in the background and writes them to the log file.
    /// </summary>
    private void ProcessLogQueue()
    {
        using var writer = new StreamWriter(_logFilePath, append: true, Encoding.UTF8) { AutoFlush = true };

        while (true)
        {
            _logSignal.WaitOne(TimeSpan.FromSeconds(1));

            while (_logQueue.TryDequeue(out var entry))
            {
                writer.WriteLine(entry);
            }
        }
    }
}
