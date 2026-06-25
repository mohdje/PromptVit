using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace PromptVit.AIClients
{
    public abstract class AIClient
    {
        readonly internal List<Prompt> prompts = [];
        readonly HttpClient httpClient;
        protected List<IAITool> tools;

        public decimal Temperature { get; set; } = 0.5M;
        public decimal TopP { get; set; } = 0.7M;
        public string ModelName { get; set; }

        /// <summary>
        /// Event raised each time a new response chunk is received after calling Invoke method with stream parameter set to true. 
        /// </summary>
        public event EventHandler<string> OnResponseStreamChunkReceived;
        /// <summary>
        /// Event raised each time a new reasoning chunk is received after calling Invoke method with stream parameter set to true. 
        /// </summary>
        public event EventHandler<string> OnReasoningStreamChunkReceived;
        protected bool streamMode;

        protected abstract string GetApiEndpointUrl();

        protected abstract void ApplyTokenHeader(HttpRequestHeaders headers, string authorizationToken);
        protected abstract void ApplyResponseFormatHeader(HttpRequestHeaders headers);

        protected abstract string BuildRequestBodyAsJson();
        internal abstract AIResponse DeserializeAIResponse(string responseContent);

        internal AIClient(string authorizationToken)
        {
            this.httpClient = new HttpClient();
            ApplyTokenHeader(this.httpClient.DefaultRequestHeaders, authorizationToken);
        }

        /// <summary>
        /// Sends a user prompt to the LLM.
        /// </summary>
        /// <param name="userPrompt">The user prompt to send</param>
        /// <param name="stream">If true the response will be retrieved by chunks using OnResponseStreamChunkReceived event. If model is reasoning it will be catch by chunks by OnReasoningStreamChunkReceived event</param>
        /// <returns>LLM response as string</returns>
        /// <exception cref="ArgumentNullException">The ModelName needs to be non empty string</exception>
        public async Task<string> Invoke(string userPrompt, bool stream = false)
        {
            if (string.IsNullOrEmpty(ModelName))
                throw new ArgumentNullException("ModelName must be specified");

            prompts.Add(new Prompt
            {
                Role = "user",
                Message = userPrompt
            });

            streamMode = stream;
            var response = await GetResponse();

            prompts.Add(new Prompt
            {
                Role = "assistant",
                Message = response
            });
            return response;
        }

        /// <summary>
        /// Set system prompt to send to the LLM.
        /// </summary>
        /// <param name="systemPrompt">The system prompt to send</param>
        public void SetSystemPrompt(string systemPrompt)
        {
            this.prompts.RemoveAll(p => p.Role == "system");
            this.prompts.Insert(0, new Prompt
            {
                Role = "system",
                Message = systemPrompt
            });
        }

        /// <summary>
        /// Remove all prompts in the history (system, user and assistant)
        /// </summary>
        public void ClearPrompts()
        {
            this.prompts.Clear();
        }

        /// <summary>
        /// Save the chat history in a text file 
        /// </summary>
        /// <param name="historyFilePath"></param>
        /// <returns></returns>
        public async Task SaveHistoryAsync(string historyFilePath)
        {
            var json = JsonSerializer.Serialize(this.prompts);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            await File.WriteAllTextAsync(historyFilePath, base64);
        }

        /// <summary>
        /// Load the chat history from a text file 
        /// </summary>
        /// <param name="historyFilePath"></param>
        /// <returns></returns>
        public async Task LoadHistoryAsync(string historyFilePath)
        {
            var base64 = await File.ReadAllTextAsync(historyFilePath);
            var json = Convert.FromBase64String(base64);

            this.prompts.Clear();

            var prompts = JsonSerializer.Deserialize<List<Prompt>>(json);
            if (prompts?.Any() == true)
                this.prompts.AddRange(prompts);
        }

        /// <summary>
        /// Set the tools list to send to the LLM.
        /// </summary>
        /// <param name="aiTools"></param>
        public void SetTools(IEnumerable<IAITool> aiTools)
        {
            tools = [.. aiTools];
        }

        private async Task<string> GetResponse()
        {
            try
            {
                var requestBodyAsJson = BuildRequestBodyAsJson();
                var content = new StringContent(
                    requestBodyAsJson,
                    Encoding.UTF8,
                    "application/json"
                );

                ApplyResponseFormatHeader(httpClient.DefaultRequestHeaders);

                var response = await httpClient.PostAsync(GetApiEndpointUrl(), content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"AI request failed with status code {response.StatusCode}: {errorContent}");
                }

                if (streamMode)
                    return await DeserializeStreamResponse(response);
                else
                    return await DeserializeResponse(response);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> DeserializeResponse(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var aiResponse = DeserializeAIResponse(responseContent);

            if (aiResponse.ToolCalls?.Length != 0)
            {
                await HandleToolCalling(aiResponse.ToolCalls);
                return await GetResponse();
            }
            else
            {
                return aiResponse.Message;
            }
        }


        private async Task<string> DeserializeStreamResponse(HttpResponseMessage response)
        {
            try
            {
                await using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = new StreamReader(stream, Encoding.UTF8);

                string? line;
                var sb = new StringBuilder();
                var tools = new List<AIToolCall>();

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if (!line.StartsWith("data: "))
                        return string.Empty;

                    string data = line["data: ".Length..].Trim();

                    if (data == "[DONE]")
                        break;

                    var aiResponse = DeserializeAIResponse(data);

                    if (aiResponse?.ToolCalls?.Length > 0)
                        tools.AddRange(aiResponse.ToolCalls);

                    if (!string.IsNullOrEmpty(aiResponse?.Reasoning))
                        OnReasoningStreamChunkReceived?.Invoke(this, aiResponse?.Reasoning);

                    if (!string.IsNullOrEmpty(aiResponse?.Message))
                    {
                        OnResponseStreamChunkReceived?.Invoke(this, aiResponse?.Message);
                        sb.Append(aiResponse?.Message);
                    }
                }

                if (tools.Count > 0)
                {
                    await HandleToolCalling([.. tools]);
                    return await GetResponse();
                }
                else
                    return sb.ToString();
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private async Task HandleToolCalling(AIToolCall[] toolCalls)
        {
            prompts.Add(new Prompt
            {
                Role = "assistant",
                ToolCalls = [.. toolCalls.Select(t => new AIToolCall(t.Name, t.Arguments, t.Id))]
            });
            foreach (var toolToCall in toolCalls)
            {

                var tool = tools.FirstOrDefault(t => t.Name == toolToCall.Name);
                if (tool != null)
                {
                    string jsonResult = await tool.ExecuteToolAsync(toolToCall.Arguments);

                    prompts.Add(new Prompt
                    {
                        Role = "tool",
                        Message = jsonResult,
                        Id = toolToCall.Id
                    });
                }
            }
        }
    }
}

