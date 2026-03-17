using PromptVit.AIClients.OpenAI;

namespace PromptVit.AIClients.HuggingFace
{
    public class HuggingFaceClient(string authorizationToken) : OpenAIClient(authorizationToken)
    {
        protected override string GetApiEndpointUrl() => "https://router.huggingface.co/v1/chat/completions";
    }
}

