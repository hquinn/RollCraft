using LitePrimitives;
using RollCraft.Full.Nodes;
using RollCraft.Full.Tokens;

namespace RollCraft.Full.TokenHandlers;

internal sealed class PlusTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidPrefix", "Invalid prefix found", reader.Position));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Add(left, right));
    }
}