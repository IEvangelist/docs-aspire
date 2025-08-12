internal static partial class Program
{
    internal static void CustomEndpoint(string[] args)
    {
        // <customendpoint>
        var builder = DistributedApplication.CreateBuilder(args);

        // Custom endpoint example
        var openai = builder.AddOpenAI("openai")
                            .WithEndpoint("https://my-gateway.example.com/v1");

        var chatModel = openai.AddModel("chat", "gpt-4o-mini")
                             .WithHealthCheck();

        var api = builder.AddProject<Projects.Api>("api")
                         .WithReference(chatModel);

        builder.Build().Run();
        // </customendpoint>
    }
}