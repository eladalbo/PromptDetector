using OpenAI.Chat;
using OpenAI;
using PromptDetector.Exceptions;
using PromptDetector.Domain.Services;
using PromptDetector.Domain.Models;

namespace PromptDetector.Services
{
    public class OpenAiClient : IAiClient
    {
        private readonly ILogger<OpenAIClient> logger;
        private readonly EnvConfig envConfig;
        private readonly OpenAIClient openAIClient;
                
        public OpenAiClient(EnvConfig envConfig, ILogger<OpenAIClient> logger)
        {         
            this.envConfig = envConfig;
            this.logger = logger;

            System.ClientModel.ApiKeyCredential apiKeyCredential = new System.ClientModel.ApiKeyCredential(envConfig.ApiKey);
            OpenAIClientOptions options = new OpenAIClientOptions { Endpoint = new Uri(envConfig.BaseUrl) };
            openAIClient = new OpenAIClient(apiKeyCredential, options);
        }

        public async Task<string> DetectPrompt(string prompt)
        {
            try
            {
                ChatCompletion completion = await openAIClient.GetChatClient("gpt-4o").CompleteChatAsync(prompt);

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        return completion.Content.FirstOrDefault()?.Text ?? string.Empty;
                    case ChatFinishReason.Length:
                        throw new NotImplementedException("Incomplete model output due to MaxTokens parameter or token limit exceeded.");

                    case ChatFinishReason.ContentFilter:
                        throw new NotImplementedException("Omitted content due to a content filter flag.");

                    case ChatFinishReason.FunctionCall:
                        throw new NotImplementedException("Deprecated in favor of tool calls.");

                    default:
                        throw new NotImplementedException(completion.FinishReason.ToString());
                }                                                                
            }
            catch (Exception ex) 
            {
                logger.LogError($"Failed to call OpenAiClient. Message - {ex.Message}");
                throw new ExternalCallException();
            }            
        }
    }
}
