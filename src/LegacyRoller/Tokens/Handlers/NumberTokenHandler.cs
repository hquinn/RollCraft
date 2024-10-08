using LegacyRoller.Errors;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class NumberTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Number(token.Value));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError(
                "InvalidOperator", $"Invalid operator '{token.TokenDetails.TokenType}' at position {reader.Position - 1}"
                , reader.Position - 1));
    }
}