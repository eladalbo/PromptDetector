using PromptDetector.Domain.Models;
using PromptDetector.Domain.Repositories;

namespace PromptDetector.Data
{
    public class AuditLogsRepository : IAuditLogsRepository
    {
        private List<AuditLog> auditLogs;
        public AuditLogsRepository()
        {
            auditLogs = new List<AuditLog>();
        }
        public Task AddAuditLog(AuditLog auditLog)
        {
            auditLogs.Add(auditLog);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<AuditLog>> GetAllAuditLogs()
        {
            return auditLogs;
        }
    }
}
