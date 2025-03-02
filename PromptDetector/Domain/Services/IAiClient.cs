namespace PromptDetector.Domain.Services
{
    public interface IAiClient
    {
        Task<string> DetectPrompt(string prompt);
    }
}
