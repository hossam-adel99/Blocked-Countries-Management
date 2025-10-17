using System.Collections.Concurrent;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class LogService : ILogService
    {
        private readonly ConcurrentQueue<LogEntry> _logs = new();

        public void Add(LogEntry entry)
        {
            _logs.Enqueue(entry);
            // optional: trim to last N entries if memory concern
        }

        public IEnumerable<LogEntry> GetLogs() => _logs.ToArray().OrderByDescending(l => l.TimestampUtc);
    }

}