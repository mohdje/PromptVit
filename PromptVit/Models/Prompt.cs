namespace PromptVit;

internal class Prompt
{
    public string Role { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Id { get; set; }

    public AIToolCall[]? ToolCalls { get; set; }
}

