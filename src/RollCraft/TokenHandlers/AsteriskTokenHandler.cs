using System.Numerics;
using LitePrimitives;
using RollCraft.Helpers;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class AsteriskTokenHandler : ITokenHandler
{
    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader) 
        where TNumber : INumber<TNumber>
    {
        return ErrorHelpers.Create("Parsing.InvalidPrefix", "Invalid prefix found", reader.Position);
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Success(new Multiply<TNumber>(left, right));
    }
}