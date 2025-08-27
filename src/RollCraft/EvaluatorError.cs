namespace RollCraft;

public readonly record struct EvaluatorError : IRollError
{
    public EvaluatorError(string errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
    }

    public string ErrorCode { get; }
    public string Message { get; }
}