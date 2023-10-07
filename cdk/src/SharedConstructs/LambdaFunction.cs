using Amazon.CDK;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Lambda.Destinations;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.SQS;
using Constructs;
using XaasKit.CDK.AWS.Lambda.DotNet;

namespace SharedConstructs;

public class LambdaFunctionProps : FunctionProps
{
    public LambdaFunctionProps(string codePath)
    {
        CodePath = codePath;
    }
    
    public bool IsNativeAot { get; set; }
    
    public string CodePath { get; set; }
    
    public string Postfix { get; set; }
}

public class LambdaFunction : Construct
{
    public Function Function { get; }
    
    public Alias FunctionAlias { get; }
    
    public LambdaFunction(Construct scope, string id, LambdaFunctionProps props) : base(scope, id)
    {
        if (props.IsNativeAot)
        {
            this.Function = new Function(this, id, new FunctionProps()
            {
                FunctionName = id,
                Runtime = Runtime.PROVIDED_AL2,
                MemorySize = props.MemorySize ?? 1024,
                LogRetention = RetentionDays.ONE_DAY,
                Handler = "bootstrap",
                Environment = props.Environment,
                Tracing = Tracing.ACTIVE,
                Code = Code.FromAsset(props.CodePath),
                Architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture == System.Runtime.InteropServices.Architecture.Arm64 ? Architecture.ARM_64 : Architecture.X86_64,
                OnFailure = new SqsDestination(new Queue(this, $"{id}FunctionDLQ")),
            });
        }
        else
        {
            this.Function = new DotNetFunction(this, id, new DotNetFunctionProps()
            {
                FunctionName = id,
                Runtime = Runtime.DOTNET_6,
                MemorySize = props.MemorySize ?? 1024,
                LogRetention = RetentionDays.ONE_DAY,
                Timeout = Duration.Seconds(29),
                Handler = props.Handler,
                Environment = props.Environment,
                Tracing = Tracing.ACTIVE,
                ProjectDir = props.CodePath,
                Architecture  = Architecture.X86_64,
                OnFailure = new SqsDestination(new Queue(this, $"{id}FunctionDLQ")),
            });   
        }
    }
}