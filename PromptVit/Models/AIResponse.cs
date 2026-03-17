namespace PromptVit
{
    internal class AIResponse(string message, AIToolCall[] toolCalls)
    {
        public string Message { get; } = message;
        public AIToolCall[] ToolCalls { get; } = toolCalls;
    }

    internal class AIToolCall(string name, string jsonArgs, string id)
    {
        public string Name { get; } = name;
        public string Id { get; } = id;

        public string Arguments { get; } = jsonArgs;
    }
}
