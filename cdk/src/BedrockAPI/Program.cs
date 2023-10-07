using Amazon.CDK;
using Bedrock.API;

var app = new App();

var postFix = System.Environment.GetEnvironmentVariable("STACK_POSTFIX");

var stockPriceStack = new BedrockApiStack(
    app,
    $"BedrockApiStack{postFix}",
    new BedrockApiStackProps(
        postFix));

app.Synth();