using System.Runtime.CompilerServices;
using LegacyRoller.Errors;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal class LeftParenthesisTokenHandler : ITokenHandler
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        // Parse the expression inside the parentheses
        var expressionResult = DiceExpressionParser.ParseExpression(ref reader);

        if (expressionResult.IsFailure)
        {
            return expressionResult;
        }

        // Expect a closing ')'
        if (!reader.TryConsume(out var closingToken) || closingToken.TokenDetails.TokenType != TokenType.RightParenthesis)
        {
            return Result<DiceExpression>.Failure(
                new ParserError("ExpectedClosingParen", "Expected closing parenthesis", reader.Position));
        }

        return expressionResult;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Failure(
            new ParserError("InvalidInfixOperator", "Left parenthesis cannot be used as infix operator", reader.Position));
    }
}