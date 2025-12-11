using System.Numerics;
using MonadCraft;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class VariableTokenHandler : ITokenHandler
{
    public Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return Result<IRollError, DiceExpression<TNumber>>.Success(new Variable<TNumber>(token.VariableName!));
    }

    public Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return new ParserError(
            "Parsing.InvalidOperator",
            $"Invalid operator '{token.TokenDetails.TokenType}' at position {reader.Position - 1}",
            reader.Position - 1);
    }
}
