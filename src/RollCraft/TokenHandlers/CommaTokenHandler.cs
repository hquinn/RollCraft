using System.Numerics;
using MonadCraft;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class CommaTokenHandler : ITokenHandler
{
    public Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return new ParserError(
            "Parsing.UnexpectedComma",
            "Unexpected ',' - commas are only valid inside function calls like if()",
            reader.Position - 1);
    }

    public Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return new ParserError(
            "Parsing.UnexpectedComma",
            "Unexpected ',' - commas are only valid inside function calls like if()",
            reader.Position - 1);
    }
}
