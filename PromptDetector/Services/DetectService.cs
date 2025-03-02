using PromptDetector.Domain.Models;
using PromptDetector.Domain.Services;
using System.Reflection;

namespace PromptDetector.Services
{
    public class DetectService : IDetectService
    {
        private readonly ILogger<DetectService> logger;
        private readonly IAiClient aiClient;
        private readonly IAuditLogService auditLogService;

        public DetectService(IAiClient aiClient,IAuditLogService auditLogService, ILogger<DetectService> logger)
        {
            this.aiClient = aiClient;
            this.auditLogService = auditLogService;
            this.logger = logger;
        }

        public async Task<DetectionResponse> GetDetectionResponse(DetectionRequest detectionRequest)
        {            
            string[] topics = GetTruePropertyNames(detectionRequest.settings);
            DetectionResponse detectionResponse = new DetectionResponse();
            string aiResponse;

            string joinedTopics = string.Join(",", GetTruePropertyNames(detectionRequest.settings)); 
            string allRelevantTopicsPrompt = $"Please answer me if the given prompt is related to the following topics - {joinedTopics}. " +
                $"prompt - {detectionRequest.prompt}." +
                $"The answer must contatin only the relevant topics from the given topics, in the format of topics seperated by comma (,). " +
                $"If the prompt not realted to any of the given topics - return empty response";
            try
            {
                aiResponse = await aiClient.DetectPrompt(allRelevantTopicsPrompt);

                if (!string.IsNullOrEmpty(aiResponse))
                {
                    List<string> detectedTopics = aiResponse.Split(',').Where(dt => topics.Contains(dt)).ToList();

                    if (detectedTopics.Count >= 0)
                        detectionResponse.detected_topics = detectedTopics;
                    else
                        logger.LogError("No matching topics");
                }
                else
                {
                    logger.LogError("No matching topics");
                }

                return detectionResponse;
            }
            finally
            {
                auditLogService.AddAuditLog(detectionRequest, detectionResponse, DetectionType.detect);
            }

        }       

        public async Task<DetectionResponse> GetFastDetectionResponse(DetectionRequest detectionRequest)
        {
            string[] topics = GetTruePropertyNames(detectionRequest.settings);

            DetectionResponse detectionResponse = new DetectionResponse();
            List<Task<string>> tasks = new List<Task<string>>();
            foreach (string topic in topics)
            {
                string isTopicRelevantPrompt = $"Please answer me if the given prompt is related to this topic - {topic}. " +
                                                $"prompt - {detectionRequest.prompt}." +
                                                $"answer with 'no' when it's not related and with the given topic when it is related";
                tasks.Add(aiClient.DetectPrompt(isTopicRelevantPrompt));
            }

            try
            {
                var result = await FirstTaskToSatisfyCondition<string>(tasks, result => !result.Equals("no", StringComparison.CurrentCultureIgnoreCase) && topics.Contains(result.ToLower()));
                detectionResponse.detected_topics = new List<string> { result };
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError("No matching topics");
            }

            auditLogService.AddAuditLog(detectionRequest, detectionResponse, DetectionType.protect);
            return detectionResponse;

        }

        #region private methods
        private async Task<T> FirstTaskToSatisfyCondition<T>(List<Task<T>> tasks, Func<T, bool> condition)
        {

            while (tasks.Any())
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);

                if (!completedTask.IsCompletedSuccessfully) continue;

                var result = await completedTask;
                if (condition(result))
                {
                    return result;
                }
            }

            throw new InvalidOperationException("No task result satisfies the condition.");
        }

        private string[] GetTruePropertyNames(settings settingsInstance)
        {
            var truePropertyNames = settingsInstance.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.PropertyType == typeof(bool) && (bool)prop.GetValue(settingsInstance))
                .Select(prop => prop.Name.ToLower())
                .ToArray();

            return truePropertyNames;
        }

        #endregion
    }
}
