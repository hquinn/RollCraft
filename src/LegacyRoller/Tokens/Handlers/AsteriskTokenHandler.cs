using System.Runtime.CompilerServices;
using LegacyRoller.Errors;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class AsteriskTokenHandler : ITokenHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidPrefix", "Invalid prefix found", reader.Position));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Multiply(left, right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInfixPrecedence()
    {
        return 2;
    }
}