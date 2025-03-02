using PromptDetector.Domain.Models;

namespace PromptDetector.Domain.Services
{
    public interface IAuditLogService
    {
        Task AddAuditLog(DetectionRequest detectionRequest, DetectionResponse detectionResponse, DetectionType type);
        Task<IEnumerable<AuditLog>> GetAuditLogs();
    }
}
