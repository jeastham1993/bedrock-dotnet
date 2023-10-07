namespace Bedrock.API;

class Model
{
    public string Nickname;
    public string Id;
    public string Body;
    public Model(string nickname, string id, string body) { Nickname = nickname; Id = id; Body = body; }
}