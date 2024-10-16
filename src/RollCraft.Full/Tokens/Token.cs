namespace RollCraft.Full.Tokens;

public readonly struct Token
{
    internal readonly TokenDetails TokenDetails;
    internal readonly double Value;
    
    internal Token(TokenType tokenType, TokenCategory tokenCategory, byte prefixPrecedence, byte infixPrecedence)
    {
        TokenDetails = new TokenDetails(tokenType, tokenCategory, prefixPrecedence, infixPrecedence);
        Value = 0;
    }
    
    internal Token(TokenDetails tokenDetails)
    {
        TokenDetails = tokenDetails;
        Value = 0;
    }
    
    internal Token(double value)
    {
        TokenDetails = new TokenDetails(TokenType.Number, TokenCategory.Operand, 0, 0);
        Value = value;
    }
}

public readonly struct TokenDetails
{
    internal readonly TokenType TokenType;
    internal readonly TokenCategory TokenCategory;
    internal readonly byte PrefixPrecedence;
    internal readonly byte InfixPrecedence;
    
    internal TokenDetails(TokenType tokenType, TokenCategory tokenCategory, byte prefixPrecedence, byte infixPrecedence)
    {
        TokenType = tokenType;
        TokenCategory = tokenCategory;
        PrefixPrecedence = prefixPrecedence;
        InfixPrecedence = infixPrecedence;
    }
}