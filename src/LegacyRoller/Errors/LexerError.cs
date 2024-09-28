using LitePrimitives;

namespace LegacyRoller.Errors;

public class LexerError : IError
{
    public LexerError(string code, string message, int position)
    {
        Code = code;
        Message = message;
        Metadata.Add("Position", position);
    }

    public string Code { get; }
    public string Message { get; }
    public Severity Severity { get; } = Severity.Error;
    public Dictionary<string, object> Metadata { get; } = new();
}