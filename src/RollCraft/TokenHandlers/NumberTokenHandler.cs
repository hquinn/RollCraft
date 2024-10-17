using System.Numerics;
using LitePrimitives;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class NumberTokenHandler : ITokenHandler
{
    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Success(new Number<TNumber>(token.Value));
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Failure(
            new ParserError(
                "InvalidOperator", $"Invalid operator '{token.TokenDetails.TokenType}' at position {reader.Position - 1}"
                , reader.Position - 1));
    }
}