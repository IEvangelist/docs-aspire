﻿namespace AspireApp.Tests;

[TestClass]
public class EnvVarTests
{
    [TestMethod]
    public async Task VerifyEnvironmentVariablesAreSetTest()
    {
        // Arrange
        var appHost =
            await DistributedApplicationTestingBuilder.CreateAsync<Projects.AspireApp_AppHost>();

        var frontend = (IResourceWithEnvironment)appHost.Resources
            .Single(static r => r.Name == "webfrontend");

        var envVars = await frontend.GetEnvironmentVariableValuesAsync(
            DistributedApplicationOperation.Publish);

        CollectionAssert.Contains(envVars,
            new KeyValuePair<string, string>(
                key: "services__apiservice__https__0",
                value: "{apiservice.bindings.https.url}"));
    }
}
