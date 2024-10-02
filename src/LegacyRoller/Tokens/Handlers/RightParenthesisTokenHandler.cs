using System.Runtime.CompilerServices;
using LegacyRoller.Errors;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class RightParenthesisTokenHandler : ITokenHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("UnexpectedRightParen", "Unexpected closing parenthesis", reader.Position));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("UnexpectedRightParen", "Unexpected closing parenthesis", reader.Position));
    }
}