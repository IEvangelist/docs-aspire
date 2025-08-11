using Microsoft.Extensions.DependencyInjection;
using OpenAI;

namespace MyService.Services;

public class ExampleService
{
    private readonly OpenAIClient _chatClient;
    private readonly OpenAIClient _embeddingClient;

    public ExampleService(
        [FromKeyedServices("chat")] OpenAIClient chatClient,
        [FromKeyedServices("embeddings")] OpenAIClient embeddingClient)
    {
        _chatClient = chatClient;
        _embeddingClient = embeddingClient;
    }

    public async Task<string> GetChatResponseAsync(string prompt)
    {
        var chatClient = _chatClient.GetChatClient("gpt-4o-mini");
        var result = await chatClient.CompleteChatAsync(prompt);
        return result.Value.Content[0].Text;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        var embeddingClient = _embeddingClient.GetEmbeddingClient("text-embedding-3-small");
        var result = await embeddingClient.GenerateEmbeddingAsync(text);
        return result.Value.Vector.ToArray();
    }
}
