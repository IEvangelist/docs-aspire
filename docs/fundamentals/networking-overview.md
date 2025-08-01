---
title: .NET Aspire inner loop networking overview
description: Learn how .NET Aspire handles networking and endpoints, and how you can use them in your app code.
ms.date: 07/11/2025
ms.topic: overview
---

# .NET Aspire inner-loop networking overview

One of the advantages of developing with .NET Aspire is that it enables you to develop, test, and debug cloud-native apps locally. Inner-loop networking is a key aspect of .NET Aspire that allows your apps to communicate with each other in your development environment. In this article, you learn how .NET Aspire handles various networking scenarios with proxies, endpoints, endpoint configurations, and launch profiles.

## Networking in the inner loop

The inner loop is the process of developing and testing your app locally before deploying it to a target environment. .NET Aspire provides several tools and features to simplify and enhance the networking experience in the inner loop, such as:

- **Launch profiles**: Launch profiles are configuration files that specify how to run your app locally. You can use launch profiles (such as the _launchSettings.json_ file) to define the endpoints, environment variables, and launch settings for your app.
- **Kestrel configuration**: Kestrel configuration allows you to specify the endpoints that the Kestrel web server listens on. You can configure Kestrel endpoints in your app settings, and .NET Aspire automatically uses these settings to create endpoints.
- **Endpoints/Endpoint configurations**: Endpoints are the connections between your app and the services it depends on, such as databases, message queues, or APIs. Endpoints provide information such as the service name, host port, scheme, and environment variable. You can add endpoints to your app either implicitly (via launch profiles) or explicitly by calling <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A>.
- **Proxies**: .NET Aspire automatically launches a proxy for each service binding you add to your app, and assigns a port for the proxy to listen on. The proxy then forwards the requests to the port that your app listens on, which might be different from the proxy port. This way, you can avoid port conflicts and access your app and services using consistent and predictable URLs.

## How endpoints work

A service binding in .NET Aspire involves two integrations: a **service** representing an external resource your app requires (for example, a database, message queue, or API), and a **binding** that establishes a connection between your app and the service and provides necessary information.

.NET Aspire supports two service binding types: **implicit**, automatically created based on specified launch profiles defining app behavior in different environments, and **explicit**, manually created using <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEndpoint%2A>.

Upon creating a binding, whether implicit or explicit, .NET Aspire launches a lightweight reverse proxy on a specified port, handling routing and load balancing for requests from your app to the service. The proxy is a .NET Aspire implementation detail, requiring no configuration or management concern.

To help visualize how endpoints work, consider the .NET Aspire starter templates inner-loop networking diagram:

:::image type="content" source="media/networking/networking-proxies-1x.png" lightbox="media/networking/networking-proxies.png" alt-text=".NET Aspire Starter Application template inner loop networking diagram.":::

## How container networks are managed

When you add one or more container resources, .NET Aspire creates a dedicated container bridge network to enable service discovery between containers. This bridge network is a virtual network that lets containers communicate with each other and provides a DNS server for container-to-container service discovery using DNS names.

The network's lifetime depends on the container resources:

- If all containers have a session lifetime, the network is also session-based and is cleaned up when the app host process ends.
- If any container has a persistent lifetime, the network is persistent and remains running after the app host process terminates. Aspire reuses this network on subsequent runs, allowing persistent containers to keep communicating even when the app host isn't running.

For more information on container lifetimes, see [Container resource lifetime](orchestrate-resources.md#container-resource-lifetime).

Here are the naming conventions for container networks:

- **Session networks**: `aspire-session-network-<unique-id>-<app-host-name>`
- **Persistent networks**: `aspire-persistent-network-<project-hash>-<app-host-name>`

Each app host instance gets its own network resources. The only differences are the network's lifetime and name; service discovery works the same way for both.

Containers register themselves on the network using their resource name. Aspire uses this name for service discovery between containers. For example, a `pgadmin` container can connect to a database resource named `postgres` using `postgres:5432`.

> [!NOTE]
> Host services, such as projects or other executables, don't use container networks. They rely on exposed container ports for service discovery and communication with containers. For more details on service discovery, see [service discovery overview](../service-discovery/overview.md).

## Launch profiles

When you call <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>, the app host looks for _Properties/launchSettings.json_ to determine the default set of endpoints. The app host selects a specific launch profile using the following rules:

1. An explicit `launchProfileName` argument passed when calling `AddProject`.
1. The `DOTNET_LAUNCH_PROFILE` environment variable. For more information, see [.NET environment variables](/dotnet/core/tools/dotnet-environment-variables).
1. The first launch profile defined in _launchSettings.json_.

Consider the following _launchSettings.json_ file:

:::code language="json" source="snippets/networking/Networking.Frontend/Networking.Frontend/Properties/launchSettings.json":::

For the remainder of this article, imagine that you've created an <xref:Aspire.Hosting.IDistributedApplicationBuilder> assigned to a variable named `builder` with the <xref:Aspire.Hosting.DistributedApplication.CreateBuilder> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);
```

To specify the **http** and **https** launch profiles, configure the `applicationUrl` values for both in the _launchSettings.json_ file. These URLs are used to create endpoints for this project. This is the equivalent of:

:::code source="snippets/networking/Networking.AppHost/Program.WithLaunchProfile.cs" id="verbose":::

> [!IMPORTANT]
> If there's no _launchSettings.json_ (or launch profile), there are no bindings by default.

For more information, see [.NET Aspire and launch profiles](launch-profiles.md).

## Kestrel configured endpoints

.NET Aspire supports Kestrel endpoint configuration. For example, consider an _appsettings.json_ file for a project that defines a Kestrel endpoint with the HTTPS scheme and port 5271:

:::code language="json" source="snippets/networking/Networking.Frontend/Networking.Frontend/appsettings.Development.json" highlight="8-14":::

The preceding configuration specifies an `Https` endpoint. The `Url` property is set to `https://*:5271`, which means the endpoint listens on all interfaces on port 5271. For more information, see [Configure endpoints for the ASP.NET Core Kestrel web server](/aspnet/core/fundamentals/servers/kestrel/endpoints).

With the Kestrel endpoint configured, the project should remove any configured `applicationUrl` from the _launchSettings.json_ file.

> [!NOTE]
> If the `applicationUrl` is present in the _launchSettings.json_ file and the Kestrel endpoint is configured, the app host will throw an exception.

When you add a project resource, there's an overload that lets you specify that the Kestrel endpoint should be used instead of the _launchSettings.json_ file:

:::code source="snippets/networking/Networking.AppHost/Program.KestrelConfiguration.cs" id="kestrel":::

For more information, see <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>.

## Ports and proxies

When defining a service binding, the host port is *always* given to the proxy that sits in front of the service. This allows single or multiple replicas of a service to behave similarly. Additionally, all resource dependencies that use the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> API rely of the proxy endpoint from the environment variable.

Consider the following method chain that calls <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.AddProject%2A>, <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpEndpoint%2A>, and then <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.WithReplicas%2A>:

:::code source="snippets/networking/Networking.AppHost/Program.WithReplicas.cs" id="withreplicas":::

The preceding code results in the following networking diagram:

:::image type="content" source="media/networking/proxy-with-replicas-1x.png" lightbox="media/networking/proxy-with-replicas.png" alt-text=".NET Aspire frontend app networking diagram with specific host port and two replicas.":::

The preceding diagram depicts the following:

- A web browser as an entry point to the app.
- A host port of 5066.
- The frontend proxy sitting between the web browser and the frontend service replicas, listening on port 5066.
- The `frontend_0` frontend service replica listening on the randomly assigned port 65001.
- The `frontend_1` frontend service replica listening on the randomly assigned port 65002.

Without the call to `WithReplicas`, there's only one frontend service. The proxy still listens on port 5066, but the frontend service listens on a random port:

:::code source="snippets/networking/Networking.AppHost/Program.HostPortAndRandomPort.cs" id="hostport":::

There are two ports defined:

- A host port of 5066.
- A random proxy port that the underlying service will be bound to.

:::image type="content" source="media/networking/proxy-host-port-and-random-port-1x.png" lightbox="media/networking/proxy-host-port-and-random-port.png" alt-text=".NET Aspire frontend app networking diagram with specific host port and random port.":::

The preceding diagram depicts the following:

- A web browser as an entry point to the app.
- A host port of 5066.
- The frontend proxy sitting between the web browser and the frontend service, listening on port 5066.
- The frontend service listening on random port of 65001.

The underlying service is fed this port via `ASPNETCORE_URLS` for project resources. Other resources access to this port by specifying an environment variable on the service binding:

:::code source="snippets/networking/Networking.AppHost/Program.EnvVarPort.cs" id="envvarport":::

The previous code makes the random port available in the `PORT` environment variable. The app uses this port to listen to incoming connections from the proxy. Consider the following diagram:

:::image type="content" source="media/networking/proxy-with-env-var-port-1x.png" lightbox="media/networking/proxy-with-env-var-port.png" alt-text=".NET Aspire frontend app networking diagram with specific host port and environment variable port.":::

The preceding diagram depicts the following:

- A web browser as an entry point to the app.
- A host port of 5067.
- The frontend proxy sitting between the web browser and the frontend service, listening on port 5067.
- The frontend service listening on an environment 65001.

> [!TIP]
> To avoid an endpoint being proxied, set the `IsProxied` property to `false` when calling the `WithEndpoint` extension method. For more information, see [Endpoint extensions: additional considerations](#additional-considerations).

## Omit the host port

When you omit the host port, .NET Aspire generates a random port for both host and service port. This is useful when you want to avoid port conflicts and don't care about the host or service port. Consider the following code:

:::code source="snippets/networking/Networking.AppHost/Program.OmitHostPort.cs" id="omithostport":::

In this scenario, both the host and service ports are random, as shown in the following diagram:

:::image type="content" source="media/networking/proxy-with-random-ports-1x.png" lightbox="media/networking/proxy-with-random-ports.png" alt-text=".NET Aspire frontend app networking diagram with random host port and proxy port.":::

The preceding diagram depicts the following:

- A web browser as an entry point to the app.
- A random host port of 65000.
- The frontend proxy sitting between the web browser and the frontend service, listening on port 65000.
- The frontend service listening on a random port of 65001.

## Container ports

When you add a container resource, .NET Aspire automatically assigns a random port to the container. To specify a container port, configure the container resource with the desired port:

:::code source="snippets/networking/Networking.AppHost/Program.ContainerPort.cs" id="containerport":::

The preceding code:

- Creates a container resource named `frontend`, from the `mcr.microsoft.com/dotnet/samples:aspnetapp` image.
- Exposes an `http` endpoint by binding the host to port 8000 and mapping it to the container's port 8080.

Consider the following diagram:

:::image type="content" source="media/networking/proxy-with-docker-port-mapping-1x.png" alt-text=".NET Aspire frontend app networking diagram with a docker host.":::

## Endpoint extension methods

Any resource that implements the <xref:Aspire.Hosting.ApplicationModel.IResourceWithEndpoints> interface can use the `WithEndpoint` extension methods. There are several overloads of this extension, allowing you to specify the scheme, container port, host port, environment variable name, and whether the endpoint is proxied.

There's also an overload that allows you to specify a delegate to configure the endpoint. This is useful when you need to configure the endpoint based on the environment or other factors. Consider the following code:

:::code source="snippets/networking/Networking.AppHost/Program.WithEndpoint.cs" id="withendpoint":::

The preceding code provides a callback delegate to configure the endpoint. The endpoint is named `admin` and configured to use the `http` scheme and transport, as well as the 17003 host port. The consumer references this endpoint by name, consider the following `AddHttpClient` call:

```csharp
builder.Services.AddHttpClient<WeatherApiClient>(
    client => client.BaseAddress = new Uri("http://_admin.apiservice"));
```

The `Uri` is constructed using the `admin` endpoint name prefixed with the `_` sentinel. This is a convention to indicate that the `admin` segment is the endpoint name belonging to the `apiservice` service. For more information, see [.NET Aspire service discovery](../service-discovery/overview.md).

### Additional considerations

When calling the `WithEndpoint` extension method, the `callback` overload exposes the raw <xref:Aspire.Hosting.ApplicationModel.EndpointAnnotation>, which allows the consumer to customize many aspects of the endpoint.

The `AllocatedEndpoint` property allows you to get or set the endpoint for a service. The `IsExternal` and `IsProxied` properties determine how the endpoint is managed and exposed: `IsExternal` decides if it should be publicly accessible, while `IsProxied` ensures DCP manages it, allowing for internal port differences and replication.

> [!TIP]
> If you're hosting an external executable that runs its own proxy and encounters port binding issues due to DCP already binding the port, try setting the `IsProxied` property to `false`. This prevents DCP from managing the proxy, allowing your executable to bind the port successfully.

The `Name` property identifies the service, whereas the `Port` and `TargetPort` properties specify the desired and listening ports, respectively.

For network communication, the `Protocol` property supports **TCP** and **UDP**, with potential for more in the future, and the `Transport` property indicates the transport protocol (**HTTP**, **HTTP2**, **HTTP3**). Lastly, if the service is URI-addressable, the `UriScheme` property provides the URI scheme for constructing the service URI.

For more information, see the available properties of the [EndpointAnnotation properties](/dotnet/api/aspire.hosting.applicationmodel.endpointannotation#properties).

## Endpoint filtering

All .NET Aspire project resource endpoints follow a set of default heuristics. Some endpoints are included in `ASPNETCORE_URLS` at runtime, some are published as `HTTP/HTTPS_PORTS`, and some configurations are resolved from Kestrel configuration. Regardless of the default behavior, you can filter the endpoints that are included in environment variables by using the <xref:Aspire.Hosting.ProjectResourceBuilderExtensions.WithEndpointsInEnvironment%2A> extension method:

:::code source="snippets/networking/Networking.AppHost/Program.EndpointFilter.cs" id="filter":::

The preceding code adds a default HTTPS endpoint, as well as an `admin` endpoint on port 19227. However, the `admin` endpoint is excluded from the environment variables. This is useful when you want to expose an endpoint for internal use only.
