using System.Net.Http.Headers;
using System.Text.Json;

namespace PromptVit.AIClients.Google
{
    public class GoogleAIStudioClient(string authorizationToken) : AIClient(authorizationToken)
    {
        protected override string GetApiEndpointUrl()
        {
            var baseUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelName}:";

            if (streamMode)
                baseUrl += "streamGenerateContent?alt=sse";
            else
                baseUrl += "generateContent";

            return baseUrl;
        }
        protected override void ApplyTokenHeader(HttpRequestHeaders headers, string authorizationToken)
        {
            headers.Add("x-goog-api-key", authorizationToken);
        }

        readonly JsonSerializerOptions jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        protected override void ApplyResponseFormatHeader(HttpRequestHeaders headers)
        {
            headers.Accept.Clear();
            var mediaType = "application/json";
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        internal override AIResponse DeserializeAIResponse(string responseContent)
        {
            var responseObject = JsonSerializer.Deserialize<GoogleAIStudioResponse>(responseContent, jsonSerializerOptions);

            var message = responseObject?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

            var googleAiToolCalls = responseObject?.Candidates?.FirstOrDefault()?.Content?.Parts?.Where(p => p.FunctionCall != null);
            var aiToolCalls = googleAiToolCalls?.Select(tc =>
                new AIToolCall(
                    tc.FunctionCall.Name,
                    JsonSerializer.Serialize(tc.FunctionCall.Args),
                    tc.ThoughtSignature)) ?? [];

            return new AIResponse(message, [.. aiToolCalls]);
        }

        protected override string BuildRequestBodyAsJson()
        {
            var payload = new GoogleAIStudioPayload
            {
                GenerationConfig = new()
                {
                    TopP = TopP,
                    Temperature = Temperature
                },
                Contents = [.. BuildGoogleAIContents()]
            };

            var systemPromptMessage = prompts.FirstOrDefault(p => p.Role == "system")?.Message;
            if (!string.IsNullOrEmpty(systemPromptMessage))
            {
                payload.SystemInstruction = new()
                {
                    Parts =
                    [
                        new () { Text = prompts.FirstOrDefault(p => p.Role == "system")?.Message }
                    ]
                };
            }

            if (tools?.Count > 0)
            {
                payload.Tools = [
                    new()
                    {
                        FunctionDeclarations = [..tools.Select(t => new GoogleAIFunctionDeclaration
                        {
                            Name = t.Name,
                            Description = t.Description,
                            Parameters = new ()
                            {
                                Type = "object",
                                Properties = t.Parameters.ToDictionary(p => p.Name, p => new GoogleAIFunctionProperty
                                {
                                    Type = p.Type
                                }),

                            }
                        })]
                    }];
            }
            return JsonSerializer.Serialize(payload, jsonSerializerOptions);
        }

        private IEnumerable<GoogleAIContent> BuildGoogleAIContents()
        {
            var contents = new List<GoogleAIContent>();

            foreach (var prompt in prompts)
            {
                if (prompt.Role == "user")
                {
                    contents.Add(new GoogleAIContent
                    {
                        Role = "user",
                        Parts = [new() { Text = prompt.Message }]
                    });
                }
                else if (prompt.Role == "tool")
                {
                    contents.Add(new GoogleAIContent
                    {
                        Role = "user",
                        Parts = [new GoogleAIPart()
                            {
                                FunctionResponse = new ()
                                {
                                    Name = prompt.Id,
                                    Response = new ()
                                    {
                                        Output = prompt.Message
                                    }
                                }
                            }]
                    });
                }
                else if (prompt.Role == "assistant")
                {
                    var content = new GoogleAIContent { Role = "assistant" };
                    var parts = new List<GoogleAIPart>();

                    if (!string.IsNullOrEmpty(prompt.Message))
                        parts.Add(new() { Text = prompt.Message });

                    if (prompt.ToolCalls?.Length > 0)
                    {
                        parts.AddRange(prompt.ToolCalls.Select(t => new GoogleAIPart()
                        {
                            FunctionCall = new GoogleAIFunctionCall
                            {
                                Name = t.Name,
                                Args = JsonSerializer.Deserialize<Dictionary<string, object>>(t.Arguments, jsonSerializerOptions)
                            },
                            ThoughtSignature = t.Id
                        }));
                    }

                    content.Parts = [.. parts];
                    contents.Add(content);
                }
            }
            return contents;
        }
    }
}

