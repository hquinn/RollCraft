using LegacyRoller.Nodes;
using LegacyRoller.Tokens;
using LitePrimitives;

namespace LegacyRoller.TokenHandlers;

internal sealed class SlashTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidPrefix", "Invalid prefix found", reader.Position));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Divide(left, right));
    }
}