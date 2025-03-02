using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PromptDetector.Domain.Models;
using PromptDetector.Domain.Services;
using PromptDetector.Exceptions;
using PromptDetector.Validations;

namespace PromptDetector.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DetectionController : ControllerBase
    {
        private readonly IDetectService detectService;
        private readonly IAuditLogService auditLogService;

        public DetectionController(IDetectService detectService, IAuditLogService auditLogService)
        {
            this.detectService = detectService;
            this.auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<ActionResult> Detect([FromBody] DetectionRequest detetectionRequest)
        {
            try
            {
                detetectionRequest.IsValid();

                var detectResponse = await detectService.GetDetectionResponse(detetectionRequest);
                return Ok(detectResponse);
            }
            catch (InputValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ExternalCallException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed calling external resources");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed for unknown reason");
            }

        }

        [HttpPost]
        public async Task<ActionResult> Protect([FromBody] DetectionRequest detetectionRequest)
        {
            try
            {
                detetectionRequest.IsValid();

                var detectResponse = await detectService.GetFastDetectionResponse(detetectionRequest);                
                return Ok(detectResponse);
            }
            catch (InputValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ExternalCallException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed calling external resources");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed for unknown reason");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> Logs()
        {
            var auditLogs = await auditLogService.GetAuditLogs();

            if (auditLogs == null || auditLogs.Count() == 0)
                return NotFound("No Actions have made in the service yet");

            return Ok(auditLogs);
        }        
    }
}
