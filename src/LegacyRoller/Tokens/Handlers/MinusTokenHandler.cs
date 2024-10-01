using System.Runtime.CompilerServices;
using LegacyRoller.Errors;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class MinusTokenHandler : ITokenHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        if (!reader.TryPeek(out var nextToken))
        {
            return Result<DiceExpression>.Failure(
                new ParserError("UnexpectedEnd", "Unexpected end of input", reader.Position));
        }

        // We want to treat '-dX' as '-1dX'
        if (nextToken.TokenType == TokenType.Dice)
        {
            return Result<DiceExpression>.Success(new Unary(new Number(1)));
        }
        
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