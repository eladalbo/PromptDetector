using PromptDetector.Domain.Models;

namespace PromptDetector.Domain.Services
{
    public interface IDetectService
    {
        Task<DetectionResponse> GetDetectionResponse(DetectionRequest detectionRequest);
        Task<DetectionResponse> GetFastDetectionResponse(DetectionRequest detectionRequest);
    }
}
