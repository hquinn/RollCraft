namespace LegacyRoller.Tokens;

public readonly struct Token
{
    internal readonly TokenDetails TokenDetails;
    internal readonly double Value;
    
    internal Token(TokenType tokenType, byte prefixPrecedence, byte infixPrecedence)
    {
        TokenDetails = new TokenDetails(tokenType, prefixPrecedence, infixPrecedence);
        Value = 0;
    }
    
    internal Token(TokenDetails tokenDetails)
    {
        TokenDetails = tokenDetails;
        Value = 0;
    }
    
    internal Token(double value)
    {
        TokenDetails = new TokenDetails(TokenType.Number, 0, 0);
        Value = value;
    }
}

public readonly struct TokenDetails
{
    internal readonly TokenType TokenType;
    internal readonly byte PrefixPrecedence;
    internal readonly byte InfixPrecedence;
    internal readonly byte Padding;
    
    internal TokenDetails(TokenType tokenType, byte prefixPrecedence, byte infixPrecedence)
    {
        TokenType = tokenType;
        PrefixPrecedence = prefixPrecedence;
        InfixPrecedence = infixPrecedence;
        Padding = 0;
    }
}