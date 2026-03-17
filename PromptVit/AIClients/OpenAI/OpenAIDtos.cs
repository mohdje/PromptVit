using System.Text.Json.Serialization;

namespace PromptVit.AIClients.OpenAI
{
    public class OpenAIPayload
    {
        public decimal Temperature { get; set; }
        public decimal TopP { get; set; }
        public string Model { get; set; }
        public bool Stream { get; set; }

        public List<OpenAIMessage> Messages { get; set; }
        public List<OpenAITool>? Tools { get; set; }
    }

    public class OpenAITool
    {
        public string Type { get; set; }
        public OpenAIFunction Function { get; set; }
    }

    public class OpenAIFunction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Strict { get; set; }
        public OpenAIParameters Parameters { get; set; }
    }

    public class OpenAIParameters
    {
        public string Type { get; set; }
        public Dictionary<string, OpenAIParameterProperty> Properties { get; set; }
        public List<string> Required { get; set; } = [];
    }

    public class OpenAIParameterProperty
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class OpenAIResponse
    {
        public OpenAIChoice[] Choices { get; set; }
    }

    public class OpenAIChoice
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public OpenAIMessage? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Delta? Delta { get; set; }
    }

    public class OpenAIMessage
    {
        public string Content { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Function { get; set; }
        public string Role { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ToolCallId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OpenAIToolCall>? ToolCalls { get; set; }
    }

    public class OpenAIToolCall
    {
        public OpenAIFunctionCall Function { get; set; }
        public string Id { get; set; }

        public string Type { get; set; }
    }

    public class OpenAIFunctionCall
    {
        public string Name { get; set; }
        public string Arguments { get; set; }
    }

    public class Choice
    {
        public int Index { get; set; }
        public Delta? Delta { get; set; }
        public string? FinishReason { get; set; }
    }

    public class Delta
    {
        public string? Role { get; set; }
        public string? Content { get; set; }
        public string? Reasoning { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OpenAIToolCall>? ToolCalls { get; set; }
    }
}

