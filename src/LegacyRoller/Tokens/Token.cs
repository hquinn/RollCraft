namespace LegacyRoller.Tokens;

internal readonly struct Token
{
    internal readonly TokenType TokenType;
    internal readonly double Value;
    
    internal Token(TokenType tokenType)
    {
        TokenType = tokenType;
        Value = 0;
    }
    
    internal Token(TokenType tokenType, double value)
    {
        TokenType = tokenType;
        Value = value;
    }
}