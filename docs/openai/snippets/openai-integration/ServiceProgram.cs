using OpenAI;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAI client using the connection from AppHost
builder.AddOpenAIClient("chat");

// Add keyed clients for multiple models
builder.AddKeyedOpenAIClient("chat");
builder.AddKeyedOpenAIClient("embeddings");

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
