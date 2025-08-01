---
title: Orchestrate Node.js apps in .NET Aspire
description: Learn how to integrate Node.js and npm apps into a .NET Aspire App Host project.
ms.date: 05/27/2025
ms.custom: sfi-image-nochange
---

# Orchestrate Node.js apps in .NET Aspire

In this article, you learn how to use Node.js and Node Package Manager (`npm`) apps in a .NET Aspire project. The sample app in this article demonstrates [Angular](https://angular.io), [React](https://react.dev/), and [Vue](https://vuejs.org/) client experiences. The following .NET Aspire APIs exist to support these scenarios—and they're part of the [Aspire.Hosting.NodeJS](https://nuget.org/packages/Aspire.Hosting.NodeJS) NuGet package:

- [Node.js](https://nodejs.org/): <xref:Aspire.Hosting.NodeAppHostingExtension.AddNodeApp%2A>.
- [`npm` apps](https://docs.npmjs.com/cli/using-npm/scripts): <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp%2A>.

The difference between these two APIs is that the former is used to host Node.js apps, while the latter is used to host apps that execute from a _package.json_ file's `scripts` section—and the corresponding `npm run <script-name>` command.

> [!TIP]
> The sample source code for this article is available on [GitHub](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript), and there are details available on the [Code Samples: .NET Aspire with Angular, React and Vue](/samples/dotnet/aspire-samples/aspire-angular-react-vue) page.

> [!IMPORTANT]
> While this article is focused on Single-Page App (SPA) frontend bits, there's an additional Node.js sample available on the [Code Samples: .NET Aspire Node.js sample](/samples/dotnet/aspire-samples/aspire-nodejs) page, that demonstrates how to use Node.js as a server app with [express](https://expressjs.com/).

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

Additionally, you need to install [Node.js](https://nodejs.org/en/download/package-manager) on your machine. The sample app in this article was built with Node.js version 20.12.2 and npm version 10.5.1. To verify your Node.js and npm versions, run the following commands:

```nodejs
node --version
```

```nodejs
npm --version
```

To download Node.js (including `npm`), see the [Node.js download page](https://nodejs.org/en/download/package-manager).

## Clone sample source code

To clone the sample source code from [GitHub](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript), run the following command:

```bash
git clone https://github.com/dotnet/aspire-samples.git
```

After cloning the repository, navigate to the _samples/AspireWithJavaScript_ folder:

```bash
cd samples/AspireWithJavaScript
```

From this directory, there are six child directories described in the following list:

- **AspireJavaScript.Angular**: An Angular app that consumes the weather forecast API and displays the data in a table.
- **AspireJavaScript.AppHost**: A .NET Aspire project that orchestrates the other apps in this sample. For more information, see [.NET Aspire orchestration overview](../fundamentals/app-host-overview.md).
- **AspireJavaScript.MinimalApi**: An HTTP API that returns randomly generated weather forecast data.
- **AspireJavaScript.React**: A React app that consumes the weather forecast API and displays the data in a table.
- **AspireJavaScript.ServiceDefaults**: The default shared project for .NET Aspire projects. For more information, see [.NET Aspire service defaults](../fundamentals/service-defaults.md).
- **AspireJavaScript.Vue**: A Vue app that consumes the weather forecast API and displays the data in a table.

## Install client dependencies

The sample app demonstrates how to use JavaScript client apps that are built on top of Node.js. Each client app was written either using a `npm create` template command or manually. The following table lists the template commands used to create each client app, along with the default port:

| App type                       | Create template command       | Default port |
|--------------------------------|-------------------------------|--------------|
| [Angular](https://angular.dev) | `npm create @angular@latest`  | 4200         |
| [React](https://react.dev)     | Didn't use a template.        | PORT env var |
| [Vue](https://vuejs.org)       | `npm create vue@latest`       | 5173         |

> [!TIP]
> You don't need to run any of these commands, since the sample app already includes the clients. Instead, this is a point of reference from which the clients were created. For more information, see [npm-init](https://docs.npmjs.com/cli/commands/npm-init).

To run the app, you first need to install the dependencies for each client. To do so, navigate to each client folder and run [`npm install` (or the install alias `npm i`) commands](https://docs.npmjs.com/cli/v10/commands/npm-install).

### Install Angular dependencies

```nodejs
npm i ./AspireJavaScript.Angular/
```

For more information on the Angular app, see [explore the Angular client](#explore-the-angular-client).

### Install React dependencies

```nodejs
npm i ./AspireJavaScript.React/
```

For more information on the React app, see [explore the React client](#explore-the-react-client).

### Install Vue dependencies

```nodejs
npm i ./AspireJavaScript.Vue/
```

For more information on the Vue app, see [explore the Vue client](#explore-the-vue-client).

## Run the sample app

To run the sample app, call the [dotnet run](/dotnet/core/tools/dotnet-run) command given the orchestrator app host _AspireJavaScript.AppHost.csproj_ as the `--project` switch:

```dotnetcli
dotnet run --project ./AspireJavaScript.AppHost/AspireJavaScript.AppHost.csproj
```

The [.NET Aspire dashboard](../fundamentals/dashboard/overview.md) launches in your default browser, and each client app endpoint displays under the **Endpoints** column of the **Resources** page. The following image depicts the dashboard for this sample app:

:::image type="content" source="media/aspire-dashboard-with-nodejs.png" lightbox="media/aspire-dashboard-with-nodejs.png" alt-text=".NET Aspire dashboard with multiple JavaScript client apps.":::

The `weatherapi` service endpoint resolves to a Swagger UI page that documents the HTTP API. Each client app consumes this service to display the weather forecast data. You can view each client app by navigating to the corresponding endpoint in the .NET Aspire dashboard. Their screenshots and the modifications made from the template starting point are detailed in the following sections.

In the same terminal session that you used to run the app, press <kbd>Ctrl</kbd> + <kbd>C</kbd> to stop the app.

## Explore the app host

To help understand how each client app resource is orchestrated, look to the app host project. The app host requires the [Aspire.Hosting.NodeJS](https://nuget.org/packages/Aspire.Hosting.NodeJS) NuGet package to host Node.js apps:

:::code language="xml" highlight="15,22-30" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.AppHost/AspireJavaScript.AppHost.csproj":::

The project file also defines a build target that ensures that the npm dependencies are installed before the app host is built. The app host code (_Program.cs_) declares the client app resources using the <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp(Aspire.Hosting.IDistributedApplicationBuilder,System.String,System.String,System.String,System.String[])> API.

:::code source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.AppHost/AppHost.cs":::

The preceding code:

- Creates a <xref:Aspire.Hosting.DistributedApplicationBuilder>.
- Adds the "weatherapi" service as a project to the app host.
  - Marks the HTTP endpoints as external.
- With a reference to the "weatherapi" service, adds the "angular", "react", and "vue" client apps as npm apps.
  - Each client app is configured to run on a different container port, and uses the `PORT` environment variable to determine the port.
  - All client apps also rely on a _Dockerfile_ to build their container image and are configured to express themselves in the publishing manifest as a container from the <xref:Aspire.Hosting.ExecutableResourceBuilderExtensions.PublishAsDockerFile*> API.

For more information on inner-loop networking, see [.NET Aspire inner-loop networking overview](../fundamentals/networking-overview.md). For more information on deploying apps, see [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md).

When the app host orchestrates the launch of each client app, it uses the `npm run start` command. This command is defined in the `scripts` section of the _package.json_ file for each client app. The `start` script is used to start the client app on the specified port. Each client app relies on a proxy to request the "weatherapi" service.

The proxy is configured in:

- The _proxy.conf.js_ file for the Angular client.
- The _webpack.config.js_ file for the React client.
- The _vite.config.ts_ file for the Vue client.

## Explore the Angular client

There are several key modifications from the original Angular template. The first is the addition of a _proxy.conf.js_ file. This file is used to proxy requests from the Angular client to the "weatherapi" service.

:::code language="javascript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/proxy.conf.js":::

The .NET Aspire app host sets the `services__weatherapi__http__0` environment variable, which is used to resolve the "weatherapi" service endpoint. The preceding configuration proxies HTTP requests that start with `/api` to the target URL specified in the environment variable.

Then include the proxy file to in the _angular.json_ file.
Update the `serve` target to include the `proxyConfig` option, referencing to the created _proxy.conf.js_ file.
The Angular CLI will now use the proxy configuration while serving the Angular client app.

:::code language="javascript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/angular.json" range="59-73" highlight="13":::

The third update is to the _package.json_ file. This file is used to configure the Angular client to run on a different port than the default port. This is achieved by using the `PORT` environment variable, and the `run-script-os` npm package to set the port.

:::code language="json" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/package.json":::

The `scripts` section of the _package.json_ file is used to define the `start` script. This script is used by the `npm start` command to start the Angular client app. The `start` script is configured to use the `run-script-os` package to set the port, which delegates to the `ng serve` command passing the appropriate `--port` switch based on the OS-appropriate syntax.

In order to make HTTP calls to the "weatherapi" service, the Angular client app needs to be configured to provide the Angular `HttpClient` for dependency injection. This is achieved by using the `provideHttpClient` helper function while configuring the application in the _app.config.ts_ file.

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.config.ts":::

Finally, the Angular client app needs to call the `/api/WeatherForecast` endpoint to retrieve the weather forecast data. There are several HTML, CSS, and TypeScript updates, all of which are made to the following files:

- _app.component.css_: [Update the CSS to style the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.css)
- _app.component.html_: [Update the HTML to display the weather forecast data in a table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.html)
- _app.component.ts_: [Update the TypeScript to call the `/api/WeatherForecast` endpoint and display the data in the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.ts)

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/src/app/app.component.ts":::

### Angular app running

To visualize the Angular client app, navigate to the "angular" endpoint in the .NET Aspire dashboard. The following image depicts the Angular client app:

:::image type="content" source="media/angular-app.png" lightbox="media/angular-app.png" alt-text="Angular client app with fake forecast weather data displayed as a table.":::

## Explore the React client

The React app wasn't written using a template, and instead was written manually. The complete source code can be found in the [dotnet/aspire-samples repository](https://github.com/dotnet/aspire-samples/tree/main/samples/AspireWithJavaScript/AspireJavaScript.React). Some of the key points of interest are found in the _src/App.js_ file:

:::code language="javascript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.React/src/components/App.js":::

The `App` function is the entry point for the React client app. It uses the `useState` and `useEffect` hooks to manage the state of the weather forecast data. The `fetch` API is used to make an HTTP request to the `/api/WeatherForecast` endpoint. The response is then converted to JSON and set as the state of the weather forecast data.

:::code language="javascript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.React/webpack.config.js":::

The preceding code defines the `module.exports` as follows:

- The `entry` property is set to the _src/index.js_ file.
- The `devServer` relies on a proxy to forward requests to the "weatherapi" service, sets the port to the `PORT` environment variable, and allows all hosts.
- The `output` results in a _dist_ folder with a _bundle.js_ file.
- The `plugins` set the _src/index.html_ file as the template, and expose the _favicon.ico_ file.

The final updates are to the following files:

- _App.css_: [Update the CSS to style the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.React/src/App.css)
- _App.js_: [Update the JavaScript to call the `/api/WeatherForecast` endpoint and display the data in the table.](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.React/src/App.js)

### React app running

To visualize the React client app, navigate to the "react" endpoint in the .NET Aspire dashboard. The following image depicts the React client app:

:::image type="content" source="media/react-app.png" lightbox="media/react-app.png" alt-text="React client app with fake forecast weather data displayed as a table.":::

## Explore the Vue client

There are several key modifications from the original Vue template. The primary updates were the addition of the `fetch` call in the _TheWelcome.vue_ file to retrieve the weather forecast data from the `/api/WeatherForecast` endpoint. The following code snippet demonstrates the `fetch` call:

:::code language="html" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Vue/src/components/TheWelcome.vue":::

As the `TheWelcome` integration is `mounted`, it calls the `/api/weatherforecast` endpoint to retrieve the weather forecast data. The response is then set as the `forecasts` data property. To set the server port, the Vue client app uses the `PORT` environment variable. This is achieved by updating the _vite.config.ts_ file:

:::code language="typescript" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Vue/vite.config.ts":::

Additionally, the Vite config specifies the `server.proxy` property to forward requests to the "weatherapi" service. This is achieved by using the `services__weatherapi__http__0` environment variable, which is set by the .NET Aspire app host.

The final update from the template is made to the _TheWelcome.vue_ file. This file calls the `/api/WeatherForecast` endpoint to retrieve the weather forecast data, and displays the data in a table. It includes [CSS, HTML, and TypeScript updates](https://github.com/dotnet/aspire-samples/blob/ef6868b0999c6eea3d42a10f2b20433c5ea93720/samples/AspireWithJavaScript/AspireJavaScript.Vue/src/components/TheWelcome.vue).

### Vue app running

To visualize the Vue client app, navigate to the "vue" endpoint in the .NET Aspire dashboard. The following image depicts the Vue client app:

:::image type="content" source="media/vue-app.png" lightbox="media/vue-app.png" alt-text="Vue client app with fake forecast weather data displayed as a table.":::

## Deployment considerations

The sample source code for this article is designed to run locally. Each client app deploys as a container image. The _Dockerfile_ for each client app is used to build the container image. Each _Dockerfile_ is identical, using a multistage build to create a production-ready container image.

:::code language="dockerfile" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/Dockerfile":::

The client apps are currently configured to run as true SPA apps, and aren't configured to run in a server-side rendered (SSR) mode. They sit behind **nginx**, which is used to serve the static files. They use a _default.conf.template_ file to configure **nginx** to proxy requests to the client app.

:::code language="nginx" source="~/aspire-samples/samples/AspireWithJavaScript/AspireJavaScript.Angular/default.conf.template":::

## Node.js server app considerations

While this article focuses on client apps, you might have scenarios where you need to host a Node.js server app. The same semantics are required to host a Node.js server app as a SPA client app. The .NET Aspire app host requires a package reference to the [Aspire.Hosting.NodeJS](https://nuget.org/packages/Aspire.Hosting.NodeJS) NuGet package and the code needs to call either `AddNodeApp` or `AddNpmApp`. These APIs are useful for adding existing JavaScript apps to the .NET Aspire app host.

When configuring secrets and passing environment variables to JavaScript-based apps, whether they are client or server apps, use parameters. For more information, see [.NET Aspire: External parameters—secrets](../fundamentals/external-parameters.md#secret-values).

### Use the OpenTelemetry JavaScript SDK

To export OpenTelemetry logs, traces, and metrics from a Node.js server app, you use the [OpenTelemetry JavaScript SDK](https://opentelemetry.io/docs/languages/js/).

For a complete example of a Node.js server app using the OpenTelemetry JavaScript SDK, you can refer to the [Code Samples: .NET Aspire Node.js sample](/samples/dotnet/aspire-samples/aspire-nodejs) page. Consider the sample's _instrumentation.js_ file, which demonstrates how to configure the OpenTelemetry JavaScript SDK to export logs, traces, and metrics:

:::code language="javascript" source="~/aspire-samples/samples/AspireWithNode/NodeFrontend/instrumentation.js":::

> [!TIP]
> To configure the .NET Aspire dashboard OTEL CORS settings, see the [.NET Aspire dashboard OTEL CORS settings](../fundamentals/dashboard/configuration.md#otlp-cors) page.

## Summary

While there are several considerations that are beyond the scope of this article, you learned how to build .NET Aspire projects that use Node.js and Node Package Manager (`npm`). You also learned how to use the <xref:Aspire.Hosting.NodeAppHostingExtension.AddNpmApp%2A> APIs to host Node.js apps and apps that execute from a _package.json_ file, respectively. Finally, you learned how to use the `npm` CLI to create Angular, React, and Vue client apps, and how to configure them to run on different ports.

## See also

- [Code Samples: .NET Aspire with Angular, React, and Vue](/samples/dotnet/aspire-samples/aspire-angular-react-vue)
- [Code Samples: .NET Aspire Node.js App](/samples/dotnet/aspire-samples/aspire-nodejs)
