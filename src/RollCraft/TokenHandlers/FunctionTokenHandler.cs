using System.Numerics;
using MonadCraft;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class FunctionTokenHandler : ITokenHandler
{
    public Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        var functionType = token.TokenDetails.TokenType switch
        {
            TokenType.Floor => FunctionType.Floor,
            TokenType.Ceil => FunctionType.Ceil,
            TokenType.Round => FunctionType.Round,
            TokenType.FuncMin => FunctionType.Min,
            TokenType.FuncMax => FunctionType.Max,
            TokenType.Abs => FunctionType.Abs,
            TokenType.Sqrt => FunctionType.Sqrt,
            _ => (FunctionType?)null
        };

        if (functionType is null)
        {
            return new ParserError("Parsing.InvalidFunction", $"Unknown function token type: {token.TokenDetails.TokenType}", reader.Position);
        }

        // Expect opening parenthesis
        if (!reader.TryConsume(out var openParen) || openParen.TokenDetails.TokenType != TokenType.LeftParenthesis)
        {
            return new ParserError("Parsing.ExpectedOpenParen", $"Expected '(' after function name", reader.Position);
        }

        var arguments = new List<DiceExpression<TNumber>>();
        var expectingArg = true;

        while (true)
        {
            if (reader.TryPeek(out var nextToken) && nextToken.TokenDetails.TokenType == TokenType.RightParenthesis)
            {
                reader.TryConsume(out _);
                break;
            }

            if (!expectingArg)
            {
                // Expect comma
                if (!reader.TryConsume(out var comma) || comma.TokenDetails.TokenType != TokenType.Comma)
                {
                    return new ParserError("Parsing.ExpectedCommaOrCloseParen", "Expected ',' or ')' in function call", reader.Position);
                }
            }

            var argResult = DiceExpressionParser.ParseExpression(ref reader);
            if (argResult.IsFailure)
            {
                return argResult;
            }

            arguments.Add(argResult.Value);
            expectingArg = false;
        }

        // Validate argument count for functions that require specific counts
        var minArgs = GetMinArgumentCount(functionType.Value);
        var maxArgs = GetMaxArgumentCount(functionType.Value);

        if (arguments.Count < minArgs)
        {
            return new ParserError("Parsing.TooFewArguments", $"Function '{functionType.Value.ToString().ToLowerInvariant()}' requires at least {minArgs} argument(s)", reader.Position);
        }

        if (maxArgs.HasValue && arguments.Count > maxArgs.Value)
        {
            return new ParserError("Parsing.TooManyArguments", $"Function '{functionType.Value.ToString().ToLowerInvariant()}' accepts at most {maxArgs.Value} argument(s)", reader.Position);
        }

        return Result<IRollError, DiceExpression<TNumber>>.Success(
            new Function<TNumber>(functionType.Value, arguments.ToArray()));
    }

    public Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right,
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        return new ParserError("Parsing.InvalidInfixOperator", "Functions cannot be used as infix operators", reader.Position);
    }

    private static int GetMinArgumentCount(FunctionType functionType)
    {
        return functionType switch
        {
            FunctionType.Floor => 1,
            FunctionType.Ceil => 1,
            FunctionType.Round => 1,
            FunctionType.Min => 2,
            FunctionType.Max => 2,
            FunctionType.Abs => 1,
            FunctionType.Sqrt => 1,
            _ => 1
        };
    }

    private static int? GetMaxArgumentCount(FunctionType functionType)
    {
        return functionType switch
        {
            FunctionType.Floor => 1,
            FunctionType.Ceil => 1,
            FunctionType.Round => 1,
            FunctionType.Min => null, // Unlimited
            FunctionType.Max => null, // Unlimited
            FunctionType.Abs => 1,
            FunctionType.Sqrt => 1,
            _ => null
        };
    }
}
