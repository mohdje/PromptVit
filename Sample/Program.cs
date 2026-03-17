using PromptVit;
using PromptVit.AIClients;

var aiClient = PromptVitFactory.CreateHuggingFaceClient(Tokens.HuggingFaceToken, "openai/gpt-oss-20b");

await StartChatWithTools(aiClient);

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

        await aiClient.Invoke(userInput, true);
    }
}

static async Task StartSimpleChatWithResponseStream(AIClient aiClient)
{
    Console.WriteLine("Start chatting with AI");

    aiClient.SetSystemPrompt("You are an AI assistant that gives live information about the weather. Use the available tools to get the live information.");

    var chunkReceived = false;
    aiClient.OnStreamChunkReceived += async (s, chunk) =>
    {
        if (!chunkReceived)
        {
            WriteColored("Assistant", ConsoleColor.Green);
            Console.Write("> ");
            chunkReceived = true;
        }
        Console.Write(chunk);
    };

    while (true)
    {
        WriteColored("User", ConsoleColor.Blue);
        Console.Write("> ");
        var userInput = Console.ReadLine();

        Console.WriteLine("AI Thinking...");

        chunkReceived = false;
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



