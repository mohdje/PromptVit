namespace PromptVit
{
    internal class AIResponse(string? message, AIToolCall[] toolCalls, string? reasoning = null)
    {
        public string? Message { get; } = message;
        public AIToolCall[] ToolCalls { get; } = toolCalls;
        public string? Reasoning { get; } = reasoning;
    }

    internal class AIToolCall(string name, string jsonArguments, string? id = null)
    {
        public string Name { get; } = name;
        public string? Id { get; } = id;

        public string JsonArguments { get; } = jsonArguments;
    }
}
