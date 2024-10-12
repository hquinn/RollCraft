using LegacyRoller.Tokens;
using LitePrimitives;

namespace LegacyRoller.TokenHandlers;

internal sealed class RightParenthesisTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("UnexpectedRightParen", "Unexpected closing parenthesis", reader.Position));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("UnexpectedRightParen", "Unexpected closing parenthesis", reader.Position));
    }
}