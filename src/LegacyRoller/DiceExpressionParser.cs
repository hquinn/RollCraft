using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LegacyRoller.Errors;
using LegacyRoller.Nodes;
using LegacyRoller.Tokens;
using LitePrimitives;

namespace LegacyRoller;

public static class DiceExpressionParser
{
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

    private static Result<DiceExpression> ParseExpression(ref TokenReader reader, int precedence = 0)
    {
        if (!reader.TryConsume(out var token))
        {
            return Result<DiceExpression>.Failure(
                new ParserError("UnexpectedEnd", "Unexpected end of input", reader.Position));
        }

        var leftResult = ParsePrefix(token, ref reader);
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

            if (precedence >= GetInfixPrecedence(nextToken))
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

    private static Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        switch (token.TokenType)
        {
            case TokenType.Number:
                return Result<DiceExpression>.Success(new Number(token.Value));
            case TokenType.Minus:
            {
                var precedence = GetPrefixPrecedence(token);
                var rightResult = ParseExpression(ref reader, precedence);
                return rightResult.IsFailure
                    ? rightResult
                    : Result<DiceExpression>.Success(new Unary(rightResult.Value!));
            }
            case TokenType.Plus:
            case TokenType.Asterisk:
            case TokenType.Slash:
            default:
                return Result<DiceExpression>.Failure(
                    new ParserError(
                        "InvalidToken", $"Unexpected token '{token.TokenType}' at position {reader.Position - 1}"
                        , reader.Position - 1));
        }
    }

    private static Result<DiceExpression> ParseInfix(DiceExpression left, Token token, ref TokenReader reader)
    {
        var precedence = GetInfixPrecedence(token);
        var rightResult = ParseExpression(ref reader, precedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        return token.TokenType switch
        {
            TokenType.Plus => Result<DiceExpression>.Success(new Add(left, rightResult.Value!)),
            TokenType.Minus => Result<DiceExpression>.Success(new Subtract(left, rightResult.Value!)),
            TokenType.Asterisk => Result<DiceExpression>.Success(new Multiply(left, rightResult.Value!)),
            TokenType.Slash => Result<DiceExpression>.Success(new Divide(left, rightResult.Value!)),
            _ => Result<DiceExpression>.Failure(
                new ParserError(
                    "InvalidOperator", $"Invalid operator '{token.TokenType}' at position {reader.Position - 1}"
                    , reader.Position - 1))
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetInfixPrecedence(Token token)
    {
        return token.TokenType switch
        {
            TokenType.Plus => 1,
            TokenType.Minus => 1,
            TokenType.Asterisk => 2,
            TokenType.Slash => 2,
            _ => 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetPrefixPrecedence(Token token)
    {
        return token.TokenType switch
        {
            TokenType.Minus => 3,
            _ => 0
        };
    }
}