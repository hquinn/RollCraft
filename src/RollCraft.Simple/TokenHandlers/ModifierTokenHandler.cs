using LitePrimitives;
using RollCraft.Simple.Tokens;

namespace RollCraft.Simple.TokenHandlers;

internal sealed class ModifierTokenHandler : ITokenHandler
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