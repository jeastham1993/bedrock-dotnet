using Amazon.BedrockRuntime;
using Amazon.Lambda.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Bedrock.API;

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(new AmazonBedrockRuntimeClient());
    }
}