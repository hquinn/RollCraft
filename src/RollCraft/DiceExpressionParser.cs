using System.Numerics;
using System.Runtime.InteropServices;
using LitePrimitives;
using RollCraft.Lexing;
using RollCraft.TokenHandlers;
using RollCraft.Tokens;

namespace RollCraft;

public class DiceExpressionParser
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
    
    private static Result<List<Token<TNumber>>> Tokenize<TNumber>(ReadOnlySpan<char> input)
        where TNumber : INumber<TNumber>
    {
        var numberType = typeof(TNumber);
        
        return numberType switch
        {
            // not null when numberType == typeof(double) => DiceExpressionLexer.Tokenize<double, DoubleLexer>(input.AsSpan()),
            _ when numberType == typeof(int) => (Result<List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<int, IntLexer>(input),
            _ when numberType == typeof(double) => (Result<List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<double, DoubleLexer>(input),
            _ => Result<List<Token<TNumber>>>.Failure(new ParserError("InvalidNumberType", "Invalid number type", 0))
        };
    }
    
    public static Result<DiceExpression<TNumber>> Parse<TNumber>(string input)
        where TNumber : INumber<TNumber>
    {
        var tokensResult = Tokenize<TNumber>(input);

        if (tokensResult.IsFailure)
        {
            return Result<DiceExpression<TNumber>>.Failure(tokensResult.Errors!);
        }

        var tokensAsSpan = CollectionsMarshal.AsSpan(tokensResult.Value!);
        var reader = new TokenReader<TNumber>(tokensAsSpan);
        var expressionResult = ParseExpression(ref reader);

        if (expressionResult.IsFailure)
        {
            return expressionResult;
        }

        if (reader.TryPeek(out var extraToken))
        {
            return Result<DiceExpression<TNumber>>.Failure(
                new ParserError(
                    "UnexpectedToken", 
                    $"Unexpected token '{extraToken.TokenDetails.TokenType}' at position {reader.Position}", 
                    reader.Position));
        }

        return expressionResult;
    }


    internal static Result<DiceExpression<TNumber>> ParseExpression<TNumber>(ref TokenReader<TNumber> reader, byte precedence = 0)
        where TNumber : INumber<TNumber>
    {
        if (!reader.TryConsume(out var token))
        {
            return Result<DiceExpression<TNumber>>.Failure(
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

    private static Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
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