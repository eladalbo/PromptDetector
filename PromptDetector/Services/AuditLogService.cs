using PromptDetector.Domain.Models;
using PromptDetector.Domain.Repositories;
using PromptDetector.Domain.Services;

namespace PromptDetector.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IAuditLogsRepository repository;
        private readonly ILogger<AuditLogService> logger;

        public AuditLogService(IAuditLogsRepository repository, ILogger<AuditLogService> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }
     
        public async Task AddAuditLog(DetectionRequest detectionRequest, DetectionResponse detectionResponse, DetectionType type)
        {
            logger.LogInformation($"AddAuditLog with type: {type.ToString()}");
            
            var auditLog = new AuditLog
            {
                prompt = detectionRequest.prompt,
                result = detectionResponse,
                time = DateTime.Now,
                type = type
            };
            
            await repository.AddAuditLog(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogs()
        {
            logger.LogInformation($"GetAllAuditLogs requested");

            return await repository.GetAllAuditLogs();            
        }
    }
}
