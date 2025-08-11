using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;

namespace MyService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly OpenAIClient _client;

    public ChatController(OpenAIClient client)
    {
        _client = client;
    }

    [HttpPost("completion")]
    public async Task<IActionResult> GetCompletion([FromBody] string prompt)
    {
        var chatClient = _client.GetChatClient("gpt-4o-mini");
        
        var result = await chatClient.CompleteChatAsync(prompt);
        
        return Ok(result.Value.Content[0].Text);
    }
}
