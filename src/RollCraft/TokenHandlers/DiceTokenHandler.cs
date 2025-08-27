using System.Numerics;
using MonadCraft;
using RollCraft.Comparisons;
using RollCraft.Helpers;
using RollCraft.Modifiers;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class DiceTokenHandler : ITokenHandler
{
    private static readonly Max DefaultMaxComparison = new Max();
    private static readonly Min DefaultMinComparison = new Min();

    public Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        // Treat 'dX' as '1dX'
        var oneExpression = new Number<TNumber>(TNumber.One);

        var rightResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.PrefixPrecedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        var diceExpression = new Dice<TNumber>(oneExpression, rightResult.Value);
        var modifiersResult = ParseModifiers(ref reader, diceExpression.Modifiers);
        
        if (modifiersResult.IsFailure)
        {
            return Result<IRollError, DiceExpression<TNumber>>.Failure(modifiersResult.Error);
        }

        return Result<IRollError, DiceExpression<TNumber>>.Success(diceExpression);
    }

    public Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        var diceExpression = new Dice<TNumber>(left, right);
        var modifiersResult = ParseModifiers(ref reader, diceExpression.Modifiers);
        
        if (modifiersResult.IsFailure)
        {
            return Result<IRollError, DiceExpression<TNumber>>.Failure(modifiersResult.Error);
        }

        return Result<IRollError, DiceExpression<TNumber>>.Success(diceExpression);
    }

    private static Result<IRollError, Unit> ParseModifiers<TNumber>(
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
                return Result<IRollError, Unit>.Failure(modifierResult.Error);
            }

            modifiers.Add(modifierResult.Value);
        }

        return Result<IRollError, Unit>.Success(default);
    }


    private static Result<IRollError, IModifier> ParseModifier<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
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
                    return Result<IRollError, IModifier>.Failure(comparisonResult.Error);
                }

                var comparison = comparisonResult.Value ?? DefaultMaxComparison;

                return Result<IRollError, IModifier>.Success(new Exploding(comparison));
            }

            case TokenType.ReRollOnce:
            case TokenType.ReRoll:
            {
                var comparisonResult = ParseComparison(ref reader);

                if (comparisonResult.IsFailure)
                {
                    return Result<IRollError, IModifier>.Failure(comparisonResult.Error);
                }

                var comparison = comparisonResult.Value ?? DefaultMinComparison;

                return Result<IRollError, IModifier>.Success(new ReRoll(
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
                    return Result<IRollError, IModifier>.Failure(countResult.Error);
                }

                return Result<IRollError, IModifier>.Success(new Keep<TNumber>(countResult.Value,
                    token.TokenDetails.TokenType != TokenType.KeepLowest));
            }

            default:
                return new ParserError(
                    "Parsing.UnknownModifier", 
                    $"Unknown modifier '{token.TokenDetails.TokenType}'",
                    reader.Position - 1);
        }
    }
    
    private static Result<IRollError, IComparison?> ParseComparison<TNumber>(ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        if (!reader.TryPeek(out var nextToken) ||
            nextToken.TokenDetails.TokenCategory != TokenCategory.Comparison)
        {
            // Return default comparison if no comparison token is found
            return Result<IRollError, IComparison?>.Success(null);
        }

        reader.Advance(); // Consume the comparison token

        var comparisonResult = DiceExpressionParser.ParseExpression(ref reader, nextToken.TokenDetails.InfixPrecedence);
        if (comparisonResult.IsFailure)
        {
            return Result<IRollError, IComparison?>.Failure(comparisonResult.Error);
        }

        IComparison? comparison = nextToken.TokenDetails.TokenType switch
        {
            TokenType.Equal => new Equal<TNumber>(comparisonResult.Value),
            TokenType.NotEqual => new NotEqual<TNumber>(comparisonResult.Value),
            TokenType.GreaterThan => new GreaterThan<TNumber>(comparisonResult.Value),
            TokenType.GreaterThanEqual => new GreaterThanEqual<TNumber>(comparisonResult.Value),
            TokenType.LesserThan => new LesserThan<TNumber>(comparisonResult.Value),
            TokenType.LesserThanEqual => new LesserThanEqual<TNumber>(comparisonResult.Value),
            _ => null
        };

        return Result<IRollError, IComparison?>.Success(comparison);
    }
    
    private static Result<IRollError, IModifier> ParseValueModifier<TNumber>(
        ref TokenReader<TNumber> reader,
        Func<DiceExpression<TNumber>, IModifier> createModifier,
        byte precedence) where TNumber : INumber<TNumber>
    {
        var valueResult = DiceExpressionParser.ParseExpression(ref reader, precedence);
        if (valueResult.IsFailure)
        {
            return Result<IRollError, IModifier>.Failure(valueResult.Error);
        }

        var modifier = createModifier(valueResult.Value);
        return Result<IRollError, IModifier>.Success(modifier);
    }
}