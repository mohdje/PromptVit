using PromptVit.AIClients;
using PromptVit.AIClients.Cerebras;
using PromptVit.AIClients.Google;
using PromptVit.AIClients.HuggingFace;

namespace PromptVit
{
    public static class PromptVitFactory
    {
        public static AIClient CreateHuggingFaceClient(string token, string model)
        {
            return new HuggingFaceClient(token)
            {
                ModelName = model
            };
        }

        public static AIClient CreateCerebrasClient(string token, string model)
        {
            return new CerebrasClient(token)
            {
                ModelName = model
            };
        }

        public static AIClient CreateGoogleAIStudioClient(string token, string model)
        {
            return new GoogleAIStudioClient(token)
            {
                ModelName = model
            };
        }

    }
}