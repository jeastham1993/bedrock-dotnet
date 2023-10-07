using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SSM;

namespace Bedrock.API;

public record SharedLambdaProps(
    BedrockApiStackProps StackProps);