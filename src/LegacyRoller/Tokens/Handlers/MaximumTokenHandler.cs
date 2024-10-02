using System.Runtime.CompilerServices;
using LegacyRoller.Errors;
using LegacyRoller.Modifiers;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class MaximumTokenHandler : ITokenHandler
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
        if (left is Dice dice)
        {
            dice.Modifiers.Add(new Maximum(right));
            return Result<DiceExpression>.Success(dice);
        }
        
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidInfixOperator", "Maximum cannot be used as infix operator other than for a dice token", reader.Position));
    }
}