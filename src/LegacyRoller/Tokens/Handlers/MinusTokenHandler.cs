using System.Runtime.CompilerServices;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class MinusTokenHandler : ITokenHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        var operandResult = DiceExpressionParser.ParseExpression(ref reader, GetPrefixPrecedence());
        return operandResult.IsFailure 
            ? operandResult 
            : Result<DiceExpression>.Success(new Unary(operandResult.Value!));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Subtract(left, right));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetInfixPrecedence()
    {
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetPrefixPrecedence()
    {
        return 6;
    }
}