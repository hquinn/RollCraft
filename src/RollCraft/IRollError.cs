namespace RollCraft;

public interface IRollError
{
    public string ErrorCode { get; }
    public string Message { get; }
}