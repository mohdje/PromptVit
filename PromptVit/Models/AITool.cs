using System.Text.Json;

namespace PromptVit
{
    public class AITool<T, U>(string name, string description, AIToolParameter[] parameters, Func<U, Task<T>> function) : IAITool
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public AIToolParameter[] Parameters { get; } = parameters;

        readonly Func<U, Task<T>> Function = function;

        readonly JsonSerializerOptions serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public async Task<string> ExecuteToolAsync(string jsonParameters)
        {
            var args = JsonSerializer.Deserialize<U>(jsonParameters, serializerOptions);
            var result = await Function(args);
            return JsonSerializer.Serialize(result);
        }
    }

    public class AIToolParameter(string name, string description, string type)
    {
        public string Name { get; } = name;
        public string Description { get; } = description;
        public string Type { get; } = type;
    }
}