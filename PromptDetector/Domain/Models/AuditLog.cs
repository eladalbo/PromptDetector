namespace PromptDetector.Domain.Models
{
    public class AuditLog
    {
        public DateTime time { get; set; }
        public DetectionType type { get; set; }
        public string prompt { get; set; }
        public DetectionResponse result { get; set; }
    }
}
