using System.Numerics;

namespace RollCraft.Tokens;

public readonly struct Token<TNumber> where TNumber : INumber<TNumber>
{
    internal readonly TokenDetails TokenDetails;
    internal readonly TNumber Value;
    internal readonly string? VariableName;
    
    internal Token(TokenType tokenType, TokenCategory tokenCategory, byte prefixPrecedence, byte infixPrecedence)
    {
        TokenDetails = new TokenDetails(tokenType, tokenCategory, prefixPrecedence, infixPrecedence);
        Value = TNumber.Zero;
        VariableName = null;
    }
    
    internal Token(TNumber value)
    {
        TokenDetails = new TokenDetails(TokenType.Number, TokenCategory.Operand, 0, 0);
        Value = value;
        VariableName = null;
    }
    
    internal Token(string variableName)
    {
        TokenDetails = new TokenDetails(TokenType.Variable, TokenCategory.Operand, 0, 0);
        Value = TNumber.Zero;
        VariableName = variableName;
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