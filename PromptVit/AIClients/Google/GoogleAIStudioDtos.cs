using System.Text.Json.Serialization;

namespace PromptVit.AIClients.Google
{
    public class GoogleAIStudioPayload
    {
        public GoogleAIContent SystemInstruction { get; set; }
        public GoogleAIContent[] Contents { get; set; }
        public GoogleAIStudioConfig GenerationConfig { get; set; }

        public GoogleAITool[] Tools { get; set; }
    }

    public class GoogleAIContent
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Role { get; set; }
        public GoogleAIPart[] Parts { get; set; }
    }

    public class GoogleAIPart
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GoogleAIFunctionResponse? FunctionResponse { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public GoogleAIFunctionCall? FunctionCall { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ThoughtSignature { get; set; }
    }

    public class GoogleAIStudioConfig
    {
        public decimal TopP { get; set; }
        public decimal Temperature { get; set; }
    }

    public class GoogleAITool
    {
        public GoogleAIFunctionDeclaration[] FunctionDeclarations { get; set; }
    }
    public class GoogleAIFunctionDeclaration
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GoogleAIParameters Parameters { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Required { get; set; }

    }

    public class GoogleAIParameters
    {
        public string Type { get; set; }
        public Dictionary<string, GoogleAIFunctionProperty> Properties { get; set; }
    }

    public class GoogleAIFunctionProperty
    {
        public string Type { get; set; }
    }

    public class GoogleAIStudioResponse
    {
        public GoogleAICandidate[] Candidates { get; set; }

        public GoogleAIPromptFeedback? PromptFeedback { get; set; }

    }

    public class GoogleAICandidate
    {
        [JsonPropertyName("content")]
        public GoogleAIContent Content { get; set; }

        [JsonPropertyName("finishReason")]
        public string? FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("safetyRatings")]
        public List<GoogleAISafetyRating> SafetyRatings { get; set; }

        [JsonPropertyName("citationMetadata")]
        public GoogleAICitationMetadata? CitationMetadata { get; set; }
    }

    public class GoogleAIFunctionCall
    {
        public string Name { get; set; }
        public Dictionary<string, object> Args { get; set; }
    }

    public class GoogleAISafetyRating
    {
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("probability")]
        public string? Probability { get; set; }
    }

    public class GoogleAICitationMetadata
    {
        [JsonPropertyName("citations")]
        public List<GoogleAICitation>? Citations { get; set; }
    }

    public class GoogleAICitation
    {
        [JsonPropertyName("startIndex")]
        public int StartIndex { get; set; }

        [JsonPropertyName("endIndex")]
        public int EndIndex { get; set; }

        [JsonPropertyName("uri")]
        public string? Uri { get; set; }

        [JsonPropertyName("license")]
        public string? License { get; set; }
    }

    public class GoogleAIPromptFeedback
    {
        [JsonPropertyName("safetyRatings")]
        public List<GoogleAISafetyRating>? SafetyRatings { get; set; }

        [JsonPropertyName("blockReason")]
        public string? BlockReason { get; set; }

        [JsonPropertyName("blockReasonMessage")]
        public string? BlockReasonMessage { get; set; }

        [JsonPropertyName("tokenCount")]
        public int? TokenCount { get; set; }
    }

    public class GoogleAIFunctionResponse
    {
        public string Name { get; set; }
        public GoogleAIFunctionResponseOuput Response { get; set; }
    }

    public class GoogleAIFunctionResponseOuput
    {
        public string Output { get; set; }
    }
}

