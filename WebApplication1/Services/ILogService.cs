using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface ILogService
    {
        void Add(LogEntry entry);
        IEnumerable<LogEntry> GetLogs();
    }
}
