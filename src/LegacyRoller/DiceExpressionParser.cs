using System.Runtime.InteropServices;
using LegacyRoller.Errors;
using LegacyRoller.Tokens;
using LegacyRoller.Tokens.Handlers;
using LitePrimitives;

namespace LegacyRoller;

public static class DiceExpressionParser
{
    // Match the order of the TokenHandlers by TokenType
    private static readonly ITokenHandler[] TokenHandlers =
    [
        new MinusTokenHandler(),
        new PlusTokenHandler(),
        new AsteriskTokenHandler(),
        new SlashTokenHandler(),
        new NumberTokenHandler(),
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
        return ParseExpression(ref reader);
    }

    internal static Result<DiceExpression> ParseExpression(ref TokenReader reader, int precedence = 0)
    {
        if (!reader.TryConsume(out var token))
        {
            return Result<DiceExpression>.Failure(
                new ParserError("UnexpectedEnd", "Unexpected end of input", reader.Position));
        }

        var leftResult = TokenHandlers[(int) token.TokenType].ParsePrefix(token, ref reader);
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

            var nextPrecedence = TokenHandlers[(int) nextToken.TokenType].GetInfixPrecedence();
            
            if (precedence >= nextPrecedence)
            {
                break;
            }

            reader.TryConsume(out token);
            leftResult = ParseInfix(leftResult.Value!, token, ref reader);
            
            if (leftResult.IsFailure)
            {
                return leftResult;
            }
        }

        return leftResult;
    }

    private static Result<DiceExpression> ParseInfix(DiceExpression left, Token token, ref TokenReader reader)
    {
        var precedence = TokenHandlers[(int) token.TokenType].GetInfixPrecedence();
        var rightResult = ParseExpression(ref reader, precedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }
        
        return TokenHandlers[(int) token.TokenType].ParseInfix(left, rightResult.Value!, token, ref reader);
    }
}