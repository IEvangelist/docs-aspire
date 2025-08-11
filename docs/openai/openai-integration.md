---
title: OpenAI hosting integration
description: Learn how to use the .NET Aspire OpenAI hosting integration to configure OpenAI resources and models in your distributed applications.
ms.date: 8/11/2025
ms.topic: how-to
---

# .NET Aspire OpenAI hosting integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](../includes/banner.md)]

The .NET Aspire OpenAI hosting integration enables you to connect to the [OpenAI API](https://platform.openai.com/docs/introduction) or OpenAI-compatible services. It handles the configuration of connection strings, health checks, and telemetry for your OpenAI resources.

## Hosting integration

The OpenAI hosting integration models OpenAI services and models as resources in your distributed application. It supports both OpenAI.com and OpenAI-compatible endpoints.

### Complete example

The following example demonstrates adding OpenAI resources with multiple models to an AppHost:

:::code language="csharp" source="snippets/openai-integration/Program.cs":::

And the corresponding service configuration:

:::code language="csharp" source="snippets/openai-integration/ServiceProgram.cs":::

### Add OpenAI resource

To add an OpenAI resource to your AppHost, install the [Aspire.Hosting.OpenAI](https://www.nuget.org/packages/Aspire.Hosting.OpenAI) NuGet package in the AppHost project.

<a name="package"></a>

[!INCLUDE [package-reference](../includes/package-reference.md)]

```xml
<PackageReference Include="Aspire.Hosting.OpenAI"
                  Version="*" />
```

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

In your AppHost project, call <xref:Aspire.Hosting.OpenAIExtensions.AddOpenAI*> to add an OpenAI resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(openai);
```

The `AddOpenAI` method creates an <xref:Aspire.Hosting.OpenAI.OpenAIResource> that can host multiple models. You can then add specific models using the `AddModel` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");
var chatModel = openai.AddModel("chat", "gpt-4o-mini");
var embeddingModel = openai.AddModel("embeddings", "text-embedding-3-small");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(chatModel)
                       .WithReference(embeddingModel);
```

### Add OpenAI model resource

The <xref:Aspire.Hosting.OpenAI.OpenAIModelResource> represents a specific AI model hosted on OpenAI. To add a model resource, call the <xref:Aspire.Hosting.OpenAIExtensions.AddModel*> method on an existing OpenAI resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");
var chatModel = openai.AddModel("chat", "gpt-4o-mini");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(chatModel);
```

The `AddModel` method takes two parameters:

- `name`: The resource name used as the connection string name when referenced by other resources.
- `model`: The model identifier (for example, `"gpt-4o-mini"`, `"gpt-4o"`, or `"text-embedding-3-small"`).

### Available models

OpenAI supports various AI models. Some popular options include:

- `gpt-4o-mini` - Fast and efficient model for most tasks
- `gpt-4o` - Most capable multimodal model
- `gpt-4-turbo` - Previous generation high-performance model
- `gpt-3.5-turbo` - Fast and inexpensive model for simple tasks
- `text-embedding-3-small` - Small embedding model for text similarity
- `text-embedding-3-large` - Large embedding model for text similarity
- `dall-e-3` - Text-to-image generation model
- `whisper-1` - Speech-to-text model

For the most up-to-date list of available models, see the [OpenAI Models documentation](https://platform.openai.com/docs/models).

### Custom endpoints

By default, the OpenAI service endpoint is `https://api.openai.com/v1`. To use an OpenAI-compatible gateway or self-hosted endpoint, call the <xref:Aspire.Hosting.OpenAIExtensions.WithEndpoint*> method on the parent resource:

:::code language="csharp" source="snippets/openai-integration/CustomEndpoint.cs":::

Both the parent and model connection strings will include the custom endpoint.

### Add health checks

By default, .NET Aspire adds a basic health check for the OpenAI service that monitors the status page. To add a health check that verifies your specific model and API key, call the <xref:Aspire.Hosting.OpenAIExtensions.WithHealthCheck*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var openai = builder.AddOpenAI("openai");
var chatModel = openai.AddModel("chat", "gpt-4o-mini")
                     .WithHealthCheck();

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(chatModel);
```

The health check verifies that:

- The OpenAI endpoint is accessible
- The API key is valid
- The specified model is available

> [!NOTE]
> OpenAI API calls, including health checks, count toward your rate limits. Use model-specific health checks sparingly, such as when troubleshooting connectivity issues.

## Client integration

To get started with the .NET Aspire OpenAI client integration, install the [Aspire.OpenAI](https://www.nuget.org/packages/Aspire.OpenAI) NuGet package in the client-consuming project, that is, the project for the application that uses the OpenAI client.

[!INCLUDE [package-reference](../includes/package-reference.md)]

```xml
<PackageReference Include="Aspire.OpenAI"
                  Version="*" />
```

### Add OpenAI client

In the _Program.cs_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireOpenAIExtensions.AddOpenAIClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `OpenAIClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddOpenAIClient("chat");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the OpenAI model resource in the AppHost project. When using the project reference, the connection name should match the model resource name, not the parent OpenAI resource name.

You can then retrieve the `OpenAIClient` instance using dependency injection. For example, to retrieve the client from a web API controller:

:::code language="csharp" source="snippets/openai-integration/ChatController.cs":::

For more information on using the `OpenAIClient`, see [OpenAI .NET API library](https://github.com/openai/openai-dotnet).

### Add keyed OpenAI client

There might be situations where you want to register multiple `OpenAIClient` instances with different connection names. To register a keyed OpenAI client, call the <xref:Microsoft.Extensions.Hosting.AspireOpenAIExtensions.AddKeyedOpenAIClient*> method:

```csharp
builder.AddKeyedOpenAIClient("chat");
builder.AddKeyedOpenAIClient("embeddings");
```

Then you can retrieve the `OpenAIClient` instances using dependency injection. For example, to retrieve the connection from an example service:

:::code language="csharp" source="snippets/openai-integration/KeyedService.cs":::

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### OpenAI client without model resources

When no model resources are defined in your AppHost, you can create clients directly from the parent OpenAI resource and specify the model programmatically:

```csharp
// In AppHost
var openai = builder.AddOpenAI("openai");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(openai);

// In the service
builder.AddOpenAIClient("openai")
       .AddChatClient("gpt-4o-mini");
```

## Configuration

The .NET Aspire OpenAI integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddOpenAIClient()`:

```csharp
builder.AddOpenAIClient("openaiConnectionName");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "openaiConnectionName": "Endpoint=https://api.openai.com/v1;Key={your_api_key};"
  }
}
```

The `Endpoint` parameter is optional and defaults to `https://api.openai.com/v1` if omitted.

### Use configuration providers

The .NET Aspire OpenAI client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.OpenAI.OpenAISettings> and `OpenAIClientOptions` from configuration by using the `Aspire:OpenAI` key. Example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "OpenAI": {
      "DisableTracing": false,
      "ClientOptions": {
        "UserAgentApplicationId": "myapp"
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<OpenAISettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddOpenAIClient("openaiConnectionName", 
    settings => settings.DisableTracing = true);
```

You can also setup the `OpenAIClientOptions` using the optional `Action<OpenAIClientOptions>? configureOptions` parameter of the `AddOpenAIClient` method. For example, to set a custom `NetworkTimeout` value for this client:

```csharp
builder.AddOpenAIClient("openaiConnectionName", 
    configureOptions: options => options.NetworkTimeout = TimeSpan.FromSeconds(2));
```

## API key configuration

The OpenAI integration requires an API key for authentication. The hosting integration automatically creates a parameter for the API key.

### Default API key parameter

When you add an OpenAI resource, Aspire automatically creates a parameter named `{resource_name}-openai-apikey`. For a resource named "openai", the parameter would be `openai-openai-apikey`.

You can set this value in user secrets:

```json
{
  "Parameters": {
    "openai-openai-apikey": "sk-your-api-key-here"
  }
}
```

The parameter will also check for the `OPENAI_API_KEY` environment variable as a fallback.

### Custom API key parameter

You can replace the default API key parameter with a custom one by calling the <xref:Aspire.Hosting.OpenAIExtensions.WithApiKey*> method:

:::code language="csharp" source="snippets/openai-integration/CustomApiKey.cs":::

Then set the custom parameter in user secrets:

```json
{
  "Parameters": {
    "my-custom-api-key": "sk-your-api-key-here"
  }
}
```

This approach is useful when you want to share a single API key across multiple OpenAI resources or use a more descriptive parameter name.

## See also

- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
- [OpenAI API documentation](https://platform.openai.com/docs/introduction)
- [OpenAI .NET SDK](https://github.com/openai/openai-dotnet)
