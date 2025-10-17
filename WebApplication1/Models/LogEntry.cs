namespace WebApplication1.Models
{
    public class LogEntry
    {
        public string Ip { get; set; } = null!;
        public DateTime TimestampUtc { get; set; }
        public string CountryCode { get; set; } = null!;
        public bool Blocked { get; set; }
        public string UserAgent { get; set; } = null!;
    }

}
