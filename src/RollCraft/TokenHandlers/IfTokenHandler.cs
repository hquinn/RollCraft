using System.Numerics;
using MonadCraft;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class IfTokenHandler : ITokenHandler
{
    public Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        // Expect opening parenthesis
        if (!reader.TryConsume(out var openParen) || openParen.TokenDetails.TokenType != TokenType.LeftParenthesis)
        {
            return new ParserError("Parsing.ExpectedOpenParen", "Expected '(' after 'if'", reader.Position);
        }

        // Parse left side of condition
        var leftResult = DiceExpressionParser.ParseExpression(ref reader);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        // Expect comparison operator
        if (!reader.TryConsume(out var comparisonToken))
        {
            return new ParserError("Parsing.ExpectedComparison", "Expected comparison operator in if condition", reader.Position);
        }

        var conditionalOperator = comparisonToken.TokenDetails.TokenType switch
        {
            TokenType.Equal => ConditionalOperator.Equal,
            TokenType.NotEqual => ConditionalOperator.NotEqual,
            TokenType.GreaterThan => ConditionalOperator.GreaterThan,
            TokenType.GreaterThanEqual => ConditionalOperator.GreaterThanEqual,
            TokenType.LesserThan => ConditionalOperator.LessThan,
            TokenType.LesserThanEqual => ConditionalOperator.LessThanEqual,
            _ => (ConditionalOperator?)null
        };

        if (conditionalOperator is null)
        {
            return new ParserError(
                "Parsing.InvalidComparison", 
                $"Expected comparison operator (=, <>, >, >=, <, <=) but found '{comparisonToken.TokenDetails.TokenType}'", 
                reader.Position);
        }

        // Parse right side of condition
        var rightResult = DiceExpressionParser.ParseExpression(ref reader);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        // Expect comma
        if (!reader.TryConsume(out var comma1) || comma1.TokenDetails.TokenType != TokenType.Comma)
        {
            return new ParserError("Parsing.ExpectedComma", "Expected ',' after condition in if expression", reader.Position);
        }

        // Parse true expression
        var trueResult = DiceExpressionParser.ParseExpression(ref reader);
        if (trueResult.IsFailure)
        {
            return trueResult;
        }

        // Expect comma
        if (!reader.TryConsume(out var comma2) || comma2.TokenDetails.TokenType != TokenType.Comma)
        {
            return new ParserError("Parsing.ExpectedComma", "Expected ',' after true value in if expression", reader.Position);
        }

        // Parse false expression
        var falseResult = DiceExpressionParser.ParseExpression(ref reader);
        if (falseResult.IsFailure)
        {
            return falseResult;
        }

        // Expect closing parenthesis
        if (!reader.TryConsume(out var closeParen) || closeParen.TokenDetails.TokenType != TokenType.RightParenthesis)
        {
            return new ParserError("Parsing.ExpectedCloseParen", "Expected ')' at end of if expression", reader.Position);
        }

        return Result<IRollError, DiceExpression<TNumber>>.Success(
            new Conditional<TNumber>(
                leftResult.Value,
                conditionalOperator.Value,
                rightResult.Value,
                trueResult.Value,
                falseResult.Value));
    }

    public Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return new ParserError(
            "Parsing.InvalidOperator",
            "'if' cannot be used as an infix operator",
            reader.Position - 1);
    }
}
