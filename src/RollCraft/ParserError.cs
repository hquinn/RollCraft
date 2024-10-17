using LitePrimitives;

namespace RollCraft;

public class ParserError : IError
{
    public ParserError(string code, string message, int position)
    {
        Code = code;
        Message = message;
        Severity = Severity.Error;
        Metadata.Add("Position", position);
    }
    
    public string Code { get; }
    public string Message { get; }
    public Severity Severity { get; }
    public Dictionary<string, object> Metadata { get; } = new();
}