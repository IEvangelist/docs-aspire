---
title: What's new in .NET Aspire 9.2
description: Learn what's new in the official general availability release of .NET Aspire 9.2.
ms.date: 04/10/2025
---

# What's new in .NET Aspire 9.2

📢 .NET Aspire 9.2 is the next minor version release of .NET Aspire; it supports:

- .NET 8.0 Long Term Support (LTS)
- .NET 9.0 Standard Term Support (STS)

If you have feedback, questions, or want to contribute to .NET Aspire, collaborate with us on [:::image type="icon" source="../media/github-mark.svg" border="false"::: GitHub](https://github.com/dotnet/aspire) or join us on [:::image type="icon" source="../media/discord-icon.svg" border="false"::: Discord](https://discord.com/invite/h87kDAHQgJ) to chat with team members.

It's important to note that .NET Aspire releases out-of-band from .NET releases. While major versions of .NET Aspire align with .NET major versions, minor versions are released more frequently. For more information on .NET and .NET Aspire version support, see:

- [.NET support policy](https://dotnet.microsoft.com/platform/support/policy): Definitions for LTS and STS.
- [.NET Aspire support policy](https://dotnet.microsoft.com/platform/support/policy/aspire): Important unique product life cycle details.

## ⬆️ Upgrade to .NET Aspire 9.2

> [!IMPORTANT]
> If you are using `azd` to deploy Azure PostgreSQL or Azure SQL Server, you now have to configure Azure Managed Identities. For more information, see [breaking changes](#-breaking-changes).

Moving between minor releases of .NET Aspire is simple:

1. In your app host project file (that is, _MyApp.AppHost.csproj_), update the [📦 Aspire.AppHost.Sdk](https://www.nuget.org/packages/Aspire.AppHost.Sdk) NuGet package to version `9.2.0`:

    ```diff
    <Project Sdk="Microsoft.NET.Sdk">

        <Sdk Name="Aspire.AppHost.Sdk" Version="9.2.0" />
        
        <PropertyGroup>
            <OutputType>Exe</OutputType>
            <TargetFramework>net9.0</TargetFramework>
    -       <IsAspireHost>true</IsAspireHost>
            <!-- Omitted for brevity -->
        </PropertyGroup>
        
        <ItemGroup>
            <PackageReference Include="Aspire.Hosting.AppHost" Version="9.2.0" />
        </ItemGroup>
    
        <!-- Omitted for brevity -->
    </Project>
    ```

    > [!IMPORTANT]
    > The `IsAspireHost` property is no longer required in the project file. For more information, see [App host project file changes](#-app-host-project-file-changes).

    For more information, see [.NET Aspire SDK](xref:dotnet/aspire/sdk).

1. Check for any NuGet package updates, either using the NuGet Package Manager in Visual Studio or the **Update NuGet Package** command in VS Code.
1. Update to the latest [.NET Aspire templates](../fundamentals/aspire-sdk-templates.md) by running the following .NET command line:

    ```dotnetcli
    dotnet new update
    ```

    > [!IMPORTANT]
    > The `dotnet new update` command updates all of your templates to the latest version.

If your app host project file doesn't have the `Aspire.AppHost.Sdk` reference, you might still be using .NET Aspire 8. To upgrade to 9.0, follow [the upgrade guide](../get-started/upgrade-to-aspire-9.md).

## 🚧 App host project file changes

The .NET Aspire app host project file no longer requires the `IsAspireHost` property. This property was moved to the `Aspire.AppHost.Sdk` SDK, therefore, you can remove it from your project file. For more information, see [dotnet/aspire issue #8144](https://github.com/dotnet/aspire/pull/8144).

## 🖇️ Dashboard resource graph & icons

The dashboard now has the ability to display a resource graph, which shows the relationships between resources in your app in an interactive view. To see the resource graph, select the "Graph" tab above the Resources table. The graph icons also match new icons in the Resources table.

For more information, see [.NET Aspire dashboard configuration](../fundamentals/dashboard/configuration.md#other).

## 🌐 HTTP-based resource command functionality

[Custom resource commands](../fundamentals/custom-resource-commands.md) now support HTTP-based functionality with the addition of the `WithHttpCommand` API, enabling you to define endpoints for tasks like database migrations or resets. These commands can be run directly from the .NET Aspire dashboard.

For more information, see [Custom HTTP commands in .NET Aspire](../fundamentals/http-commands.md).

## ➕ Database resources auto-created if missing

There's [plenty of feedback and confusion](https://github.com/dotnet/aspire/issues/7101) around the `AddDatabase` API. The name implies that it adds a database, but it didn't actually create the database. In .NET Aspire 9.2, the `AddDatabase` API now creates a database for the following hosting integrations:

| Hosting integration | API reference |
|--|--|
| [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) | <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase*> |
| [📦 Aspire.Hosting.PostgreSql](https://www.nuget.org/packages/Aspire.Hosting.PostgreSql) | <xref:Aspire.Hosting.PostgresBuilderExtensions.AddDatabase*> |

The Azure SQL and Azure PostgreSQL hosting integrations both expose an `AddDatabase` API, but they don't create a database—unless you call their respective `RunAsContainer` methods. For more information, see [Understand Azure integration APIs](../azure/integrations-overview.md#understand-azure-integration-apis).

The following hosting integrations don't currently support database creation:

- [📦 Aspire.Hosting.Milvus](https://www.nuget.org/packages/Aspire.Hosting.Milvus)
- [📦 Aspire.Hosting.MongoDb](https://www.nuget.org/packages/Aspire.Hosting.MongoDb)
- [📦 Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql)
- [📦 Aspire.Hosting.Oracle](https://www.nuget.org/packages/Aspire.Hosting.Oracle)

## 📦 Other Integration updates

- All integrations now support multiple configuration providers of the same resource type.
- Redis, Valkey, and Garnet integrations now have password support by default.
- Resources including CosmosDB, EventHubs, ServiceBus, and OpenAI now support deeplinking for parent-child relationships.

## 💔 Breaking changes

With every release, we strive to make .NET Aspire better. However, some changes may break existing functionality. The following breaking changes are introduced in .NET Aspire 9.2:

- [Breaking changes in .NET Aspire 9.2](../compatibility/9.2/index.md)
