using System.Numerics;
using LitePrimitives;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class LeftParenthesisTokenHandler: ITokenHandler
{
    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
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
            return Result<DiceExpression<TNumber>>.Failure(
                new ParserError("ExpectedClosingParen", "Expected closing parenthesis", reader.Position));
        }

        return expressionResult;
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Failure(
            new ParserError("InvalidInfixOperator", "Left parenthesis cannot be used as infix operator", reader.Position));
    }
}