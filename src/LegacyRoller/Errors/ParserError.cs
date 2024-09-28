using LitePrimitives;

namespace LegacyRoller.Errors;

public class ParserError : IError
{
    public ParserError(string code, string message, int position)
    {
        Code = code;
        Message = message;
        Metadata.Add("Position", position);
    }
    
    public string Code { get; }
    public string Message { get; }
    public Severity Severity { get; }
    public Dictionary<string, object> Metadata { get; }
}