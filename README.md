# PromptVit

**Lightweight multi-provider LLM client for .NET**

Promptly is a modern, lightweight C# library that simplifies calls to major generative AI inference providers (OpenAI, Google Gemini, Anthropic Claude, Groq, Mistral, etc.) from your .NET applications.
It offers a unified, intuitive, and high-performance API to:

 - Send simple or complex prompts
 - Handle streaming responses (Server-Sent Events)
 - Use tool/function calling in a standardized way
 - Easily switch between different providers without changing your business logic

[![NuGet](https://img.shields.io/nuget/v/Promptly.svg?logo=nuget&logoColor=white&label=NuGet)](https://www.nuget.org/packages/Promptly)
[![.NET](https://img.shields.io/badge/.NET-8.0%20%7C%209.0-blueviolet)](https://dotnet.microsoft.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## ✨ Why PromptVit ?

- **Ultra-lightweight**: no unnecessary dependencies, no heavy SDKs
- **Unified API**: same code for OpenAI, Gemini, Claude, Groq…
- **Native streaming**: full support for real-time responses
- **Standardized tool calls**: JSON Schema format compatible with most providers
- **Easy switching**: switch providers in one line of configuration
- **Strongly typed**: well-typed messages, tools, responses, and errors
- **High-performance**: designed for production use (retry, timeout, logging)

## Installation

```bash
dotnet add package PromptVit
```

## Example

### Simple chat

```c#
var aiClient = PromptVitFactory.CreateGoogleAIStudioClient("your_token", "gemini-3.1-flash-lite-preview");

aiClient.SetSystemPrompt("You are an AI assistant expert in cooking. You can answer to every questions related to cooking but cannot answer to anything else.");

var userInput = "How to make a Tiramisu ?";
var response = await aiClient.Invoke(userInput);
```
### Chat with stream

```c#
var aiClient = PromptVitFactory.CreateGoogleAIStudioClient("your_token", "gemini-3.1-flash-lite-preview");
aiClient.OnStreamChunkReceived += async (s, chunk) =>
{
    Console.Write(chunk);
};

aiClient.SetSystemPrompt("You are an AI assistant expert in cooking. You can answer to every questions related to cooking but cannot answer to anything else.");

var userInput = "How to make a Tiramisu ?";
var response = await aiClient.Invoke(userInput, true);
```

### Tool calling

```c#
var aiClient = PromptVitFactory.CreateGoogleAIStudioClient("your_token", "gemini-3.1-flash-lite-preview");

aiClient.SetSystemPrompt("You are an AI assistant that can give live weather information. You always use available tools to answer to user request.");

aiClient.SetTools([
    new AITool<string, WeatherArgs>("getWeather", "Retrieves weather for a given location",
    [
        new AIToolParameter("location", "a city location (example: Paris, France)", "string")
    ],
    async (arg) => await GetWeather(arg.Location))
]);

var userInput = "What is the weather in Madrid ?";
var response = await aiClient.Invoke(userInput);

static async Task<string> GetWeather(string location)
{
    return "the temperature is 15°C with a clear sky for the whole day";
}

class WeatherArgs
{
    public string Location { get; set; }
}
```
