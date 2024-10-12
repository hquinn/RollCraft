using System.Runtime.InteropServices;
using LegacyRoller.Errors;
using LegacyRoller.Tokens;
using LegacyRoller.Tokens.Handlers;
using LitePrimitives;

namespace LegacyRoller;

public static class DiceExpressionParser
{
    private static readonly ModifierTokenHandler ModifierTokenHandler = new();
    private static readonly ComparisonTokenHandler ComparisonTokenHandler = new();
    
    // Match the order of the TokenHandlers by TokenType
    private static readonly ITokenHandler[] TokenHandlers =
    [
        new DiceTokenHandler(),
        ModifierTokenHandler,
        ModifierTokenHandler,
        ModifierTokenHandler,
        ModifierTokenHandler,
        ModifierTokenHandler,
        ModifierTokenHandler,
        ModifierTokenHandler,
        ModifierTokenHandler,
        ComparisonTokenHandler,
        ComparisonTokenHandler,
        ComparisonTokenHandler,
        ComparisonTokenHandler,
        ComparisonTokenHandler,
        ComparisonTokenHandler,
        
        new MinusTokenHandler(),
        new PlusTokenHandler(),
        new AsteriskTokenHandler(),
        new SlashTokenHandler(),
        new NumberTokenHandler(),
        new LeftParenthesisTokenHandler(),
        new RightParenthesisTokenHandler(),
    ];
    
    public static Result<DiceExpression> Parse(string input)
    {
        var tokensResult = DiceExpressionLexer.Tokenize(input.AsSpan());

        if (tokensResult.IsFailure)
        {
            return tokensResult.Map<DiceExpression>(_ => null!);
        }

        var tokensAsSpan = CollectionsMarshal.AsSpan(tokensResult.Value!);
        var reader = new TokenReader(tokensAsSpan);
        var expressionResult = ParseExpression(ref reader);

        if (expressionResult.IsFailure)
        {
            return expressionResult;
        }

        if (reader.TryPeek(out var extraToken))
        {
            return Result<DiceExpression>.Failure(
                new ParserError(
                    "UnexpectedToken", 
                    $"Unexpected token '{extraToken.TokenDetails.TokenType}' at position {reader.Position}", 
                    reader.Position));
        }

        return expressionResult;
    }


    internal static Result<DiceExpression> ParseExpression(ref TokenReader reader, byte precedence = 0)
    {
        if (!reader.TryConsume(out var token))
        {
            return Result<DiceExpression>.Failure(
                new ParserError("UnexpectedEnd", "Unexpected end of input", reader.Position));
        }

        var leftResult = TokenHandlers[(byte) token.TokenDetails.TokenType].ParsePrefix(token, ref reader);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        while (true)
        {
            if (!reader.TryPeek(out var nextToken))
            {
                break;
            }

            var nextPrecedence = nextToken.TokenDetails.InfixPrecedence;
            
            if (precedence >= nextPrecedence)
            {
                break;
            }

            reader.Advance();
            leftResult = ParseInfix(leftResult.Value!, nextToken, ref reader);
            
            if (leftResult.IsFailure)
            {
                return leftResult;
            }
        }

        return leftResult;
    }

    private static Result<DiceExpression> ParseInfix(DiceExpression left, Token token, ref TokenReader reader)
    {
        var precedence = token.TokenDetails.InfixPrecedence;
        var rightResult = ParseExpression(ref reader, precedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }
        
        return TokenHandlers[(byte) token.TokenDetails.TokenType].ParseInfix(left, rightResult.Value!, token, ref reader);
    }
}