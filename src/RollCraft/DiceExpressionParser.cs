using System.Numerics;
using System.Runtime.InteropServices;
using MonadCraft;
using RollCraft.Lexing;
using RollCraft.TokenHandlers;
using RollCraft.Tokens;

namespace RollCraft;

/// <summary>
/// Provides methods for parsing dice expression strings into <see cref="DiceExpression{TNumber}"/> AST nodes.
/// </summary>
/// <remarks>
/// <para>
/// The parser supports standard dice notation (e.g., "2d6", "1d20+5"), modifiers (exploding, keep, reroll, min, max),
/// arithmetic operators (+, -, *, /, %), variables (e.g., "[STR]"), conditional expressions (if(cond, true, false)),
/// and math functions (floor, ceil, round, min, max, abs, sqrt).
/// </para>
/// <para>
/// Use <see cref="Parse{TNumber}"/> for result-based error handling or <see cref="TryParse{TNumber}"/> for
/// out-parameter style error handling.
/// </para>
/// </remarks>
public class DiceExpressionParser
{
    private static readonly ModifierTokenHandler ModifierTokenHandler = new();
    private static readonly ComparisonTokenHandler ComparisonTokenHandler = new();
    private static readonly FunctionTokenHandler FunctionTokenHandler = new();
    
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
        new PercentTokenHandler(),
        new NumberTokenHandler(),
        new LeftParenthesisTokenHandler(),
        new RightParenthesisTokenHandler(),
        new VariableTokenHandler(),
        new IfTokenHandler(),
        new CommaTokenHandler(),
        
        // Math functions
        FunctionTokenHandler,
        FunctionTokenHandler,
        FunctionTokenHandler,
        FunctionTokenHandler,
        FunctionTokenHandler,
        FunctionTokenHandler,
        FunctionTokenHandler,
    ];
    
    private static Result<IRollError, List<Token<TNumber>>> Tokenize<TNumber>(ReadOnlySpan<char> input)
        where TNumber : INumber<TNumber>
    {
        var numberType = typeof(TNumber);
        
        return numberType switch
        {
            _ when numberType == typeof(short) => (Result<IRollError, List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<short, ShortLexer>(input),
            _ when numberType == typeof(int) => (Result<IRollError, List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<int, IntLexer>(input),
            _ when numberType == typeof(long) => (Result<IRollError, List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<long, LongLexer>(input),
            _ when numberType == typeof(float) => (Result<IRollError, List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<float, FloatLexer>(input),
            _ when numberType == typeof(double) => (Result<IRollError, List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<double, DoubleLexer>(input),
            _ when numberType == typeof(decimal) => (Result<IRollError, List<Token<TNumber>>>)(object)DiceExpressionLexer.Tokenize<decimal, DecimalLexer>(input),
            _ => new ParserError("Parsing.InvalidNumberType", $"Unsupported number type '{numberType.Name}'. Supported types are: short, int, long, float, double, decimal.", 0)
        };
    }
    
    /// <summary>
    /// Parses a dice expression string into an AST representation.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type to use for values. Supported types are <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>.</typeparam>
    /// <param name="input">The dice expression string to parse (e.g., "2d6+5", "1d20+[STR]").</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing either the parsed <see cref="DiceExpression{TNumber}"/>
    /// on success, or an <see cref="IRollError"/> (specifically <see cref="ParserError"/>) on failure.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = DiceExpressionParser.Parse&lt;int&gt;("2d6+5");
    /// if (result.IsSuccess)
    /// {
    ///     var expression = result.Value;
    ///     // Use expression with an evaluator
    /// }
    /// </code>
    /// </example>
    public static Result<IRollError, DiceExpression<TNumber>> Parse<TNumber>(string input)
        where TNumber : INumber<TNumber>
    {
        // Validate input
        if (input is null)
        {
            return new ParserError("NULL_INPUT", "Input expression cannot be null", 0);
        }
        
        if (string.IsNullOrWhiteSpace(input))
        {
            return new ParserError("EMPTY_INPUT", "Input expression cannot be empty or whitespace only", 0);
        }
        
        var tokensResult = Tokenize<TNumber>(input);

        if (tokensResult.IsFailure)
        {
            return Result<IRollError, DiceExpression<TNumber>>.Failure(tokensResult.Error);
        }

        var tokensAsSpan = CollectionsMarshal.AsSpan(tokensResult.Value);
        var reader = new TokenReader<TNumber>(tokensAsSpan);
        var expressionResult = ParseExpression(ref reader);

        if (expressionResult.IsFailure)
        {
            return expressionResult;
        }

        if (reader.TryPeek(out var extraToken))
        {
            return new ParserError(
                    "Parsing.UnexpectedToken", 
                    $"Unexpected token '{extraToken.TokenDetails.TokenType}' at position {reader.Position}", 
                    reader.Position);
        }

        return expressionResult;
    }
    
    /// <summary>
    /// Attempts to parse a dice expression string into an AST representation using the Try pattern.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type to use for values. Supported types are <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>.</typeparam>
    /// <param name="input">The dice expression string to parse (e.g., "2d6+5", "1d20+[STR]").</param>
    /// <param name="expression">
    /// When this method returns, contains the parsed <see cref="DiceExpression{TNumber}"/> if parsing succeeded,
    /// or <c>null</c> if parsing failed.
    /// </param>
    /// <returns>
    /// <c>null</c> if parsing succeeded; otherwise, a <see cref="ParserError"/> describing the failure.
    /// </returns>
    /// <example>
    /// <code>
    /// var error = DiceExpressionParser.TryParse&lt;int&gt;("2d6+5", out var expression);
    /// if (error is null)
    /// {
    ///     // Use expression with an evaluator
    /// }
    /// else
    /// {
    ///     Console.WriteLine($"Parse error: {error.Message}");
    /// }
    /// </code>
    /// </example>
    public static ParserError? TryParse<TNumber>(string input, out DiceExpression<TNumber>? expression)
        where TNumber : INumber<TNumber>
    {
        var result = Parse<TNumber>(input);
        
        if (result.IsSuccess)
        {
            expression = result.Value;
            return null;
        }
        
        expression = null;
        return (ParserError)result.Error;
    }


    internal static Result<IRollError, DiceExpression<TNumber>> ParseExpression<TNumber>(ref TokenReader<TNumber> reader, byte precedence = 0)
        where TNumber : INumber<TNumber>
    {
        // Check depth limit to prevent stack overflow
        if (reader.Depth >= TokenReader<TNumber>.MaxDepth)
        {
            return new ParserError(
                "MAX_DEPTH_EXCEEDED",
                $"Expression nesting exceeds maximum depth of {TokenReader<TNumber>.MaxDepth}",
                reader.Position);
        }
        
        reader.Depth++;
        
        try
        {
            if (!reader.TryConsume(out var token))
            {
                return new ParserError(
                    "Parsing.UnexpectedEnd", 
                    "Unexpected end of input", 
                    reader.Position);
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
                leftResult = ParseInfix(leftResult.Value, nextToken, ref reader);
                
                if (leftResult.IsFailure)
                {
                    return leftResult;
                }
            }

            return leftResult;
        }
        finally
        {
            reader.Depth--;
        }
    }

    private static Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
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
        
        return TokenHandlers[(byte) token.TokenDetails.TokenType].ParseInfix(left, rightResult.Value, token, ref reader);
    }
}