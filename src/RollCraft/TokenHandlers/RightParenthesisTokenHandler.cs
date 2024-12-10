using System.Numerics;
using LitePrimitives;
using RollCraft.Helpers;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class RightParenthesisTokenHandler : ITokenHandler
{
    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return ErrorHelpers.Create(
            "Parsing.UnexpectedRightParen", 
            "Unexpected closing parenthesis", 
            reader.Position);
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return ErrorHelpers.Create(
            "Parsing.UnexpectedRightParen", 
            "Unexpected closing parenthesis", 
            reader.Position);
    }
}