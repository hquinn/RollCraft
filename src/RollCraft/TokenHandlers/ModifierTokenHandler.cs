using System.Numerics;
using LitePrimitives;
using RollCraft.Helpers;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class ModifierTokenHandler : ITokenHandler
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
        return ErrorHelpers.Create("Parsing.InvalidInfix", "Invalid infix found", reader.Position);
    }
}