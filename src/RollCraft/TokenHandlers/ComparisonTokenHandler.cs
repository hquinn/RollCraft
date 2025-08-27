using System.Numerics;
using MonadCraft;
using RollCraft.Helpers;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class ComparisonTokenHandler : ITokenHandler
{
    public Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return new ParserError("Parsing.InvalidPrefix", "Invalid prefix found", reader.Position);
    }

    public Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return new ParserError("Parsing.InvalidInfix", "Invalid infix found", reader.Position);
    }
}