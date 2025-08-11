using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Custom API key parameter
var apiKey = builder.AddParameter("my-custom-api-key", secret: true);
var openai = builder.AddOpenAI("openai")
                    .WithApiKey(apiKey);

// Share the same API key across multiple models
var chatModel = openai.AddModel("chat", "gpt-4o-mini");
var embeddingModel = openai.AddModel("embeddings", "text-embedding-3-small");

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(chatModel)
                       .WithReference(embeddingModel);

builder.Build().Run();
