using PromptVit.AIClients.OpenAI;

namespace PromptVit.AIClients
{
    public class HuggingFaceClient(string authorizationToken) : OpenAIClient(authorizationToken)
    {
        protected override string GetApiEndpointUrl() => "https://router.huggingface.co/v1/chat/completions";
    }
}

