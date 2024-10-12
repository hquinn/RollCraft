using LitePrimitives;

namespace LegacyRoller;

public class EvaluatorError : IError
{
    public EvaluatorError(string code, string message)
    {
        Code = code;
        Message = message;
        Severity = Severity.Error;
    }
    
    public string Code { get; }
    public string Message { get; }
    public Severity Severity { get; }
    public Dictionary<string, object> Metadata { get; } = new();
}