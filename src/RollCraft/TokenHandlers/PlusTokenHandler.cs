using System.Numerics;
using LitePrimitives;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class PlusTokenHandler : ITokenHandler
{
    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Failure(
            new ParserError("InvalidPrefix", "Invalid prefix found", reader.Position));
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Success(new Add<TNumber>(left, right));
    }
}