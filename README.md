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
