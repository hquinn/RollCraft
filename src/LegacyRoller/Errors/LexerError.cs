using LitePrimitives;

namespace LegacyRoller.Errors;

public class LexerError : IError
{
    public LexerError(string code, string message, int position)
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