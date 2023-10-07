using System.Collections.Generic;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Lambda;
using Constructs;
using SharedConstructs;

namespace Bedrock.API;

public class BedrockApiEndpoint : Construct
{
    public Function Function { get; }

    public BedrockApiEndpoint(
        Construct scope,
        string id,
        SharedLambdaProps props) : base(
        scope,
        id)
    {
        this.Function = new LambdaFunction(
            this,
            $"BedrockApiEndpoint{props.StackProps.Postfix}",
            new LambdaFunctionProps("./src/Bedrock.API/Bedrock.API")
            {
                Handler    = "Bedrock.API::Bedrock.API.Endpoints.PromptEndpoints_Prompt_Generated::Prompt",
                Environment = new Dictionary<string, string>(1)
                {
                    { "ENV", props.StackProps.Postfix ?? "dev" },
                    { "POWERTOOLS_SERVICE_NAME", $"BedrockAPI{props.StackProps.Postfix}" },
                }
            }).Function;

        this.Function.Role.AttachInlinePolicy(
            new Policy(
                this,
                "AllowBedrock",
                new PolicyProps
                {
                    Statements = new[]
                    {
                        new PolicyStatement(
                            new PolicyStatementProps
                            {
                                Actions = new[] { "bedrock:*" },
                                Resources = new[] { "*" }
                            })
                    }
                }));
    }
}