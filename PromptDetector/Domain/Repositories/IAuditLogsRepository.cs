using PromptDetector.Domain.Models;

namespace PromptDetector.Domain.Repositories
{
    public interface IAuditLogsRepository
    {
        Task AddAuditLog(AuditLog auditLog);
        Task<IEnumerable<AuditLog>> GetAllAuditLogs();
    }
}
