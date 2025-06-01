
using System.Collections.Concurrent;
using System.Text;


namespace WebServer;
public class Logger
{
    private readonly ConcurrentQueue<string> _logQueue = new();
    private readonly AutoResetEvent _logSignal = new(false);
    private readonly string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        Task.Run(ProcessLogQueue);
    }

    public void Log(string message)
    {
        _logQueue.Enqueue($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        _logSignal.Set();
    }

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
