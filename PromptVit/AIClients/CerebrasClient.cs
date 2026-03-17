using PromptVit.AIClients.OpenAI;

namespace PromptVit.AIClients.Cerebras
{
    public class CerebrasClient(string authorizationToken) : OpenAIClient(authorizationToken)
    {
        protected override string GetApiEndpointUrl() => "https://api.cerebras.ai/v1/chat/completions";
    }
}

