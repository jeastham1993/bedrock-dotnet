using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;

namespace Bedrock.API;

[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(PromptRequest))]
public partial class CustomSerializationContext : JsonSerializerContext
{
    
}