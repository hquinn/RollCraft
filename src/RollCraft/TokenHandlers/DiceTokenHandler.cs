using System.Numerics;
using LitePrimitives;
using RollCraft.Comparisons;
using RollCraft.Modifiers;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class DiceTokenHandler : ITokenHandler
{
    private static readonly Max DefaultMaxComparison = new Max();
    private static readonly Min DefaultMinComparison = new Min();

    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        // Treat 'dX' as '1dX'
        var oneExpression = new Number<TNumber>(TNumber.One);

        var rightResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.PrefixPrecedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        var diceExpression = new Dice<TNumber>(oneExpression, rightResult.Value!);
        var modifiersResult = ParseModifiers(ref reader, diceExpression.Modifiers);
        
        if (modifiersResult.IsFailure)
        {
            return Result<DiceExpression<TNumber>>.Failure(modifiersResult.Errors!);
        }

        return Result<DiceExpression<TNumber>>.Success(diceExpression);
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        var diceExpression = new Dice<TNumber>(left, right);
        var modifiersResult = ParseModifiers(ref reader, diceExpression.Modifiers);
        
        if (modifiersResult.IsFailure)
        {
            return Result<DiceExpression<TNumber>>.Failure(modifiersResult.Errors!);
        }

        return Result<DiceExpression<TNumber>>.Success(diceExpression);
    }

    private static Result<Unit> ParseModifiers<TNumber>(
        ref TokenReader<TNumber> reader, 
        List<IModifier> modifiers)
        where TNumber : INumber<TNumber>
    {
        while (true)
        {
            if (!reader.TryPeek(out var token))
            {
                break;
            }

            if (token.TokenDetails.TokenCategory != TokenCategory.Modifier)
            {
                break;
            }

            reader.Advance(); // Consume the modifier token

            var modifierResult = ParseModifier(token, ref reader);

            if (modifierResult.IsFailure)
            {
                return Result<Unit>.Failure(modifierResult.Errors!);
            }

            modifiers.Add(modifierResult.Value!);
        }

        return Result<Unit>.Success(default);
    }


    private static Result<IModifier> ParseModifier<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        switch (token.TokenDetails.TokenType)
        {
            case TokenType.Minimum:
                return ParseValueModifier(ref reader, value => new Minimum<TNumber>(value), token.TokenDetails.InfixPrecedence);

            case TokenType.Maximum:
                return ParseValueModifier(ref reader, value => new Maximum<TNumber>(value), token.TokenDetails.InfixPrecedence);
            
            case TokenType.Exploding:
            {
                var comparisonResult = ParseComparison(ref reader);

                if (comparisonResult.IsFailure)
                {
                    return Result<IModifier>.Failure(comparisonResult.Errors!);
                }

                var comparison = comparisonResult.Value ?? DefaultMaxComparison;

                return Result<IModifier>.Success(new Exploding(comparison));
            }

            case TokenType.ReRollOnce:
            case TokenType.ReRoll:
            {
                var comparisonResult = ParseComparison(ref reader);

                if (comparisonResult.IsFailure)
                {
                    return Result<IModifier>.Failure(comparisonResult.Errors!);
                }

                var comparison = comparisonResult.Value ?? DefaultMinComparison;

                return Result<IModifier>.Success(new ReRoll(
                    comparison,
                    token.TokenDetails.TokenType == TokenType.ReRollOnce));
            }

            case TokenType.Keep:
            case TokenType.KeepHighest:
            case TokenType.KeepLowest:
            {
                var countResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.InfixPrecedence);
                if (countResult.IsFailure)
                {
                    return Result<IModifier>.Failure(countResult.Errors!);
                }

                return Result<IModifier>.Success(new Keep<TNumber>(countResult.Value!,
                    token.TokenDetails.TokenType != TokenType.KeepLowest));
            }

            default:
                return Result<IModifier>.Failure(
                    new ParserError("UnknownModifier", $"Unknown modifier '{token.TokenDetails.TokenType}'",
                        reader.Position - 1));
        }
    }
    
    private static Result<IComparison?> ParseComparison<TNumber>(ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        if (!reader.TryPeek(out var nextToken) ||
            nextToken.TokenDetails.TokenCategory != TokenCategory.Comparison)
        {
            // Return default comparison if no comparison token is found
            return Result<IComparison?>.Success(null);
        }

        reader.Advance(); // Consume the comparison token

        var comparisonResult = DiceExpressionParser.ParseExpression(ref reader, nextToken.TokenDetails.InfixPrecedence);
        if (comparisonResult.IsFailure)
        {
            return Result<IComparison?>.Failure(comparisonResult.Errors!);
        }

        IComparison? comparison = nextToken.TokenDetails.TokenType switch
        {
            TokenType.Equal => new Equal<TNumber>(comparisonResult.Value!),
            TokenType.NotEqual => new NotEqual<TNumber>(comparisonResult.Value!),
            TokenType.GreaterThan => new GreaterThan<TNumber>(comparisonResult.Value!),
            TokenType.GreaterThanEqual => new GreaterThanEqual<TNumber>(comparisonResult.Value!),
            TokenType.LesserThan => new LesserThan<TNumber>(comparisonResult.Value!),
            TokenType.LesserThanEqual => new LesserThanEqual<TNumber>(comparisonResult.Value!),
            _ => null
        };

        return Result<IComparison?>.Success(comparison);
    }
    
    private static Result<IModifier> ParseValueModifier<TNumber>(
        ref TokenReader<TNumber> reader,
        Func<DiceExpression<TNumber>, IModifier> createModifier,
        byte precedence) where TNumber : INumber<TNumber>
    {
        var valueResult = DiceExpressionParser.ParseExpression(ref reader, precedence);
        if (valueResult.IsFailure)
        {
            return Result<IModifier>.Failure(valueResult.Errors!);
        }

        var modifier = createModifier(valueResult.Value!);
        return Result<IModifier>.Success(modifier);
    }
}