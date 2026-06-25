using PromptVit.AIClients.OpenAI;

namespace PromptVit.AIClients
{
    public class GroqClient(string authorizationToken) : OpenAIClient(authorizationToken)
    {
        protected override string GetApiEndpointUrl() => "https://api.groq.com/openai/v1/chat/completions";
    }
}

