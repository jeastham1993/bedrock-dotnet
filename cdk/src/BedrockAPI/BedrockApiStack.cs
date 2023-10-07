using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.Cognito;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SSM;
using Constructs;
using SharedConstructs;

namespace Bedrock.API;

public record BedrockApiStackProps(
    string Postfix);

public class BedrockApiStack : Stack
{
    internal BedrockApiStack(
        Construct scope,
        string id,
        BedrockApiStackProps apiProps,
        IStackProps props = null) : base(
        scope,
        id,
        props)
    {
        var endpointProps = new SharedLambdaProps(
            apiProps);

        var bedrockApiEndpoint = new BedrockApiEndpoint(
            this,
            "BedrockApiEndpoint",
            endpointProps);

        var api = new AuthorizedApi(
                this,
                $"StockPriceApi{apiProps.Postfix}",
                new RestApiProps
                {
                    RestApiName = $"StockPriceApi{apiProps.Postfix}"
                })
            .WithUnauthorisedEndpoint(
                "/prompt",
                HttpMethod.POST,
                bedrockApiEndpoint.Function);
        
        var apiEndpointOutput = new CfnOutput(
            this,
            $"APIEndpointOutput{apiProps.Postfix}",
            new CfnOutputProps
            {
                Value = api.Url,
                ExportName = $"ApiEndpoint{apiProps.Postfix}",
                Description = "Endpoint of the Stock price API"
            });
    }
}