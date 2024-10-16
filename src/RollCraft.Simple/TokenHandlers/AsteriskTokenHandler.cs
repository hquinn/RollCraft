using LitePrimitives;
using RollCraft.Simple.Nodes;
using RollCraft.Simple.Tokens;

namespace RollCraft.Simple.TokenHandlers;

internal sealed class AsteriskTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidPrefix", "Invalid prefix found", reader.Position));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Multiply(left, right));
    }
}