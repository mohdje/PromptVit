using PromptVit.AIClients;
using PromptVit.AIClients.Google;
using PromptVit.AIClients.OpenAI;

namespace PromptVit
{
    public static class PromptVitFactory
    {
        public static AIClient CreateOpenAIClient(string apiToken, string model)
        {
            return new OpenAIClient(apiToken)
            {
                ModelName = model
            };
        }
        public static AIClient CreateHuggingFaceClient(string apiToken, string model)
        {
            return new HuggingFaceClient(apiToken)
            {
                ModelName = model
            };
        }

        public static AIClient CreateGroqClient(string apiToken, string model)
        {
            return new GroqClient(apiToken)
            {
                ModelName = model
            };
        }

        public static AIClient CreateCerebrasClient(string apiToken, string model)
        {
            return new CerebrasClient(apiToken)
            {
                ModelName = model
            };
        }

        public static AIClient CreateGoogleAIStudioClient(string apiToken, string model)
        {
            return new GoogleAIStudioClient(apiToken)
            {
                ModelName = model
            };
        }

    }
}