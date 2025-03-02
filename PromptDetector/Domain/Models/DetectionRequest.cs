namespace PromptDetector.Domain.Models
{
    public class DetectionRequest
    {
        public string prompt { get; set; }
        public settings settings { get; set; }
    }
}
