using Acrolinx.Net.Tests;
using Microsoft.Extensions.Configuration;

public class TestBase
{
    public static TestEnvironment TestEnvironment { get; set; }

    [TestInitialize]
    public void Setup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddUserSecrets<TestBase>()
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        var url = configuration["ACROLINX_API_URL"];
        var token = configuration["ACROLINX_API_SSO_TOKEN"];
        var signature = configuration["ACROLINX_DEV_SIGNATURE"];
        var username = configuration["ACROLINX_API_USERNAME"];

        TestEnvironment = new TestEnvironment(signature, url, username, token);
    }
}
