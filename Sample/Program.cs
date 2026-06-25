using PromptVit;
using PromptVit.AIClients;

var aiClient = PromptVitFactory.CreateGroqClient(Tokens.GroqApiToken, "openai/gpt-oss-120b");
await StartSimpleChat(aiClient);

static async Task StartSimpleChat(AIClient aiClient)
{
    Console.WriteLine("Start chatting with AI");

    aiClient.SetSystemPrompt("You are an AI assistant expert in cooking. You can answer to every questions related to cooking but cannot answer to anything else.");

    while (true)
    {
        WriteColored("User", ConsoleColor.Blue);
        Console.Write("> ");
        var userInput = Console.ReadLine();

        Console.WriteLine("AI Thinking...");

        var response = await aiClient.Invoke(userInput);

        WriteColored("Assistant", ConsoleColor.Green);
        Console.WriteLine("> " + response);
    }
}

static async Task StartSimpleChatWithResponseStream(AIClient aiClient)
{
    Console.WriteLine("Start chatting with AI");

    aiClient.SetSystemPrompt("You are an AI assistant expert in cooking. You can answer to every questions related to cooking but cannot answer to anything else.");

    var reasoningChunkReceived = false;
    aiClient.OnReasoningStreamChunkReceived += async (s, chunk) =>
   {
       if (!reasoningChunkReceived)
       {
           WriteColored("Reasoning", ConsoleColor.Magenta);
           Console.Write("> ");
           reasoningChunkReceived = true;
       }
       Console.Write(chunk);
   };

    var responseChunkReceived = false;
    aiClient.OnResponseStreamChunkReceived += async (s, chunk) =>
    {
        if (!responseChunkReceived)
        {
            WriteColored("Assistant", ConsoleColor.Green);
            Console.Write("> ");
            responseChunkReceived = true;
        }
        Console.Write(chunk);
    };

    while (true)
    {
        WriteColored("User", ConsoleColor.Blue);
        Console.Write("> ");
        var userInput = Console.ReadLine();

        Console.WriteLine("AI Thinking...");

        responseChunkReceived = false;
        reasoningChunkReceived = false;
        await aiClient.Invoke(userInput, true);
    }
}

static async Task StartChatWithTools(AIClient aiClient)
{
    Console.WriteLine("Start chatting with AI");

    aiClient.SetSystemPrompt("You are an AI assistant that can give live weather information. You always use available tools to answer to user request.");
    aiClient.SetTools([
        new AITool<string, WeatherArgs>("getWeather", "Retrieves weather for a given location",
        [
            new AIToolParameter("location", "a city location (example: Paris, France)", "string")
        ],
    async (arg) => await GetWeather(arg.Location))
    ]);

    while (true)
    {
        WriteColored("User", ConsoleColor.Blue);
        Console.Write("> ");
        var userInput = Console.ReadLine();

        Console.WriteLine("AI Thinking...");

        var response = await aiClient.Invoke(userInput);

        WriteColored("Assistant", ConsoleColor.Green);
        Console.WriteLine("> " + response);
    }
}

static void WriteColored(string text, ConsoleColor color)
{
    var original = Console.ForegroundColor;
    Console.ForegroundColor = color;
    try
    {
        Console.Write(text);
    }
    finally
    {
        Console.ForegroundColor = original;
    }
}

static async Task<string> GetWeather(string location)
{
    Console.WriteLine("Calling GetWeather function with parameter: " + location);
    return "the temperature is 15°C with a clear sky for the whole day";
}

class WeatherArgs
{
    public string Location { get; set; }
}



