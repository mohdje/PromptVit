using System.Net.Http.Headers;
using System.Text.Json;

namespace PromptVit.AIClients.OpenAI
{
    public class OpenAIClient(string authorizationToken) : AIClient(authorizationToken)
    {
        protected override string GetApiEndpointUrl() => "https://api.openai.com/v1/chat/completions";
        protected override void ApplyTokenHeader(HttpRequestHeaders headers, string authorizationToken)
        {
            headers.Add("Authorization", $"Bearer {authorizationToken}");
        }

        readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

        protected override void ApplyResponseFormatHeader(HttpRequestHeaders headers)
        {
            headers.Accept.Clear();
            var mediaType = streamMode ? "text/event-stream" : "application/json";
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        internal override AIResponse DeserializeAIResponse(string responseContent)
        {
            var reponseObject = JsonSerializer.Deserialize<OpenAIResponse>(responseContent, jsonSerializerOptions);

            var choice = reponseObject?.Choices?.FirstOrDefault(c => streamMode || c.Message?.Role == "assistant");
            var content = streamMode ? (choice?.Delta?.Content ?? choice?.Delta?.Reasoning) : choice?.Message.Content;

            var toolCalls = streamMode ? choice?.Delta?.ToolCalls : choice?.Message?.ToolCalls;
            var aiToolCalls = toolCalls?.Select(tc =>
                new AIToolCall(
                    tc.Function.Name,
                    tc.Function.Arguments,
                    tc.Id)) ?? [];

            return new AIResponse(content, [.. aiToolCalls]);
        }

        protected override string BuildRequestBodyAsJson()
        {
            var payload = new OpenAIPayload
            {
                Temperature = Temperature,
                TopP = TopP,
                Model = ModelName,
                Stream = streamMode,
                Messages = [.. this.prompts.Select(p => new OpenAIMessage
                {
                    Content = p.Message,
                    Role = p.Role,
                    ToolCallId = p.Id,
                    ToolCalls = p.ToolCalls != null ? [.. p.ToolCalls.Select(t => new OpenAIToolCall {
                            Id = t.Id,
                            Type = "function",
                            Function = new OpenAIFunctionCall{
                                Arguments = t.Arguments,
                                Name = t.Name
                            }
                        }
                    )] : null
                })]
            };

            if (tools?.Count > 0)
            {
                payload.Tools = [.. this.tools.Select(t => new OpenAITool
                {
                    Type = "function",
                    Function = new OpenAIFunction
                    {
                        Name = t.Name,
                        Description = t.Description,
                        Parameters = new OpenAIParameters
                        {
                            Type = "object",
                            Properties = t.Parameters.ToDictionary(
                                p => p.Name,
                                p => new OpenAIParameterProperty
                                {
                                    Type = p.Type,
                                    Description = p.Description
                                })
                        }
                    }
                })];
            }
            return JsonSerializer.Serialize(payload, jsonSerializerOptions);
        }
    }
}

