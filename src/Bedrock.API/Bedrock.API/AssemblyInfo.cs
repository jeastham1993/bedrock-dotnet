using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;
using CustomSerializationContext = Bedrock.API.CustomSerializationContext;


[assembly: LambdaSerializer(typeof(SourceGeneratorLambdaJsonSerializer<CustomSerializationContext>))]