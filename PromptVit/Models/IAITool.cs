namespace PromptVit;

public interface IAITool
{
    Task<string> ExecuteToolAsync(string jsonParameters);
    public string Description { get; }
    public AIToolParameter[] Parameters { get; }
    string Name { get; }
}