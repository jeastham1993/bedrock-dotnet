namespace Bedrock.API.Models;

public class AnthropicClaudeV2
{
    public AnthropicClaudeV2(string prompt)
    {
        this.prompt = prompt;
    }
    
    public string prompt { get; set; }
    public int max_tokens_to_sample { get; set; } = 300;
    public double temperature { get; set; } = 0.7;
    public int top_k { get; set; } = 250;
    public double top_p { get; set; } = 0.999;
    public string[] stop_sequences { get; set; } = new[] { $"{Environment.NewLine}{Environment.NewLine}Human:" };
    public string anthropic_version { get; set; } = "bedrock-2023-05-31";
}