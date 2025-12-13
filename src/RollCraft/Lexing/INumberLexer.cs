using System.Numerics;
using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Represents the result of attempting to parse a number from input.
/// </summary>
/// <typeparam name="TNumber">The numeric type being parsed.</typeparam>
internal readonly struct NumberLexResult<TNumber>
    where TNumber : INumber<TNumber>
{
    /// <summary>
    /// Gets the parsed token, if successful.
    /// </summary>
    public Token<TNumber>? Token { get; }
    
    /// <summary>
    /// Gets whether an overflow occurred during parsing.
    /// </summary>
    public bool IsOverflow { get; }
    
    /// <summary>
    /// Gets the position where overflow occurred.
    /// </summary>
    public int OverflowPosition { get; }
    
    private NumberLexResult(Token<TNumber>? token, bool isOverflow, int overflowPosition)
    {
        Token = token;
        IsOverflow = isOverflow;
        OverflowPosition = overflowPosition;
    }
    
    /// <summary>
    /// Creates a successful result with the parsed token.
    /// </summary>
    public static NumberLexResult<TNumber> Success(Token<TNumber> token) => 
        new(token, false, 0);
    
    /// <summary>
    /// Creates a result indicating no number was found (not an error).
    /// </summary>
    public static NumberLexResult<TNumber> NoMatch => 
        new(null, false, 0);
    
    /// <summary>
    /// Creates a result indicating an overflow occurred.
    /// </summary>
    public static NumberLexResult<TNumber> Overflow(int position) => 
        new(null, true, position);
}

internal interface INumberLexer<TNumber>
    where TNumber : INumber<TNumber>
{
    static abstract NumberLexResult<TNumber> GetNumber(ReadOnlySpan<char> input, ref int refIndex);
}