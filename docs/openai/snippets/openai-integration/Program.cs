using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add OpenAI resource with models
var openai = builder.AddOpenAI("openai");
var chatModel = openai.AddModel("chat", "gpt-4o-mini");
var embeddingModel = openai.AddModel("embeddings", "text-embedding-3-small");

// Add service that uses the OpenAI models
var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(chatModel)
                       .WithReference(embeddingModel);

builder.Build().Run();
