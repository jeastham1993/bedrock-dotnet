using System.Net;
using System.Text;
using System.Text.Json;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using AWS.Lambda.Powertools.Tracing;
using Bedrock.API.Models;
using SignalR.TranslationWorker.Models;

namespace Bedrock.API.Endpoints;

public class PromptEndpoints
{
    private readonly AmazonBedrockRuntimeClient _bedrockRuntimeClient;
    private List<Model> _models;

    public PromptEndpoints(AmazonBedrockRuntimeClient bedrockRuntimeClient)
    {
        _bedrockRuntimeClient = bedrockRuntimeClient;
        _models = new List<Model>();
        _models.Add(new Model("1", "anthropic.claude-instant-v1", "{{'prompt': 'Human:{0} Assistant:', 'max_tokens_to_sample':300, 'temperature':1, 'top_k':250,'top_p':0.999, 'stop_sequences':['Human'],'anthropic_version':'bedrock-2023-05-31'}}"));
        _models.Add(new Model("2", "anthropic.claude-v1", "{{'prompt': 'Human:{0} Assistant:', 'max_tokens_to_sample':300, 'temperature':1, 'top_k':250, 'top_p':1, 'stop_sequences':['Human'],'anthropic_version':'bedrock-2023-05-31' }}"));
        _models.Add(new Model("3", "anthropic.claude-v2", "{{'prompt': 'Human:{0} Assistant:', 'temperature':0.5, 'top_p':1 , 'max_tokens_to_sample':300, 'top_k':250,'stop_sequences':['Human'] }}"));
        _models.Add(new Model("4", "ai21.j2-ultra-v1", "{{'prompt':'{0}','maxTokens':200,'temperature':0.7,'topP':1,'stopSequences':[],'countPenalty':{{'scale':0}},'presencePenalty':{{'scale':0}},'frequencyPenalty':{{'scale':0}}}}"));
        _models.Add(new Model("5", "ai21.j2-mid-v1", "{{'prompt':'{0}','maxTokens':200,'temperature':0.7,'topP':1,'stopSequences':[],'countPenalty':{{'scale':0}},'presencePenalty':{{'scale':0}},'frequencyPenalty':{{'scale':0}}}}"));
        _models.Add(new Model("6", "cohere.command-text-v14", "{{'prompt':'{0}','max_tokens':400,'temperature':0.75, 'p':0.01, 'k':0, 'stop_sequences':[], 'return_likelihoods': 'NONE'}}"));

    }
    
    [LambdaFunction]
    [RestApi(LambdaHttpMethod.Post, "/prompt")]
    [Tracing]
    public async Task<PromptResponse> Prompt([FromBody] PromptRequest req)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new CohereCommand(req.Prompt)));
        MemoryStream stream = new MemoryStream(byteArray);

        var result = await this._bedrockRuntimeClient.InvokeModelAsync(new InvokeModelRequest()
        {
            ModelId = "cohere.command-text-v14",
            Body = stream,
            ContentType = "application/json",
            Accept = "application/json"
        });

        var response = JsonSerializer.Deserialize<CohereCommandResponse>(result.Body);

        return new PromptResponse()
        {
            Response = response.Generations[0].Text
        };
    }
}