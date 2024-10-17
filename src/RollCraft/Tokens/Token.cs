using System.Numerics;

namespace RollCraft.Tokens;

public readonly struct Token<TNumber> where TNumber : INumber<TNumber>
{
    internal readonly TokenDetails TokenDetails;
    internal readonly TNumber Value;
    
    internal Token(TokenType tokenType, TokenCategory tokenCategory, byte prefixPrecedence, byte infixPrecedence)
    {
        TokenDetails = new TokenDetails(tokenType, tokenCategory, prefixPrecedence, infixPrecedence);
        Value = TNumber.Zero;
    }
    
    internal Token(TNumber value)
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