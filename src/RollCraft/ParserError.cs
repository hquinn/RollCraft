namespace RollCraft;

public readonly record struct ParserError : IRollError
{
    public ParserError(string errorCode, string message, int position)
    {
        ErrorCode = errorCode;
        Message = message;
        Position = position;
    }
    
    public string ErrorCode { get; }
    public string Message { get; }
    public int Position { get; }
}