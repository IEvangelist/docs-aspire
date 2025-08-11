using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Custom endpoint example
var openai = builder.AddOpenAI("openai")
                    .WithEndpoint("https://my-gateway.example.com/v1");

var chatModel = openai.AddModel("chat", "gpt-4o-mini")
                     .WithHealthCheck();

var myService = builder.AddProject<Projects.MyService>("myservice")
                       .WithReference(chatModel);

builder.Build().Run();
