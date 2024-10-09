using LegacyRoller.Errors;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class ComparisonTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidPrefix", "Invalid prefix found", reader.Position));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidInfix", "Invalid infix found", reader.Position));
    }
}