using LegacyRoller.Comparisons;
using LegacyRoller.Errors;
using LegacyRoller.Modifiers;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class DiceTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        // Treat 'dX' as '1dX'
        var oneExpression = new Number(1);

        var rightResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.PrefixPrecedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        var diceExpression = new Dice(oneExpression, rightResult.Value!);
        var modifiersResult = ParseModifiers(ref reader);
        
        if (modifiersResult.IsFailure)
        {
            return modifiersResult.Map<DiceExpression>(_ => null!);
        }
        
        diceExpression.Modifiers.AddRange(modifiersResult.Value!);

        return Result<DiceExpression>.Success(diceExpression);
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        var diceExpression = new Dice(left, right);
        var modifiersResult = ParseModifiers(ref reader);
        
        if (modifiersResult.IsFailure)
        {
            return modifiersResult.Map<DiceExpression>(_ => null!);
        }
        
        diceExpression.Modifiers.AddRange(modifiersResult.Value!);

        return Result<DiceExpression>.Success(diceExpression);
    }

    private static Result<List<IModifier>> ParseModifiers(ref TokenReader reader)
    {
        var modifiers = new List<IModifier>(5);

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
                return modifierResult.Map<List<IModifier>>(_ => null!);
            }
            
            modifiers.Add(modifierResult.Value!);
        }

        return Result<List<IModifier>>.Success(modifiers);
    }
    
    private static Result<IModifier> ParseModifier(Token token, ref TokenReader reader)
    {
        switch (token.TokenDetails.TokenType)
        {
            case TokenType.Minimum:
                // Parse the minimum value expression
                var minValueResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.InfixPrecedence);
                if (minValueResult.IsFailure)
                {
                    return minValueResult.Map<IModifier>(_ => null!);
                }
                return Result<IModifier>.Success(new Minimum(minValueResult.Value!));
                
            case TokenType.Maximum:
                // Parse the maximum value expression
                var maxValueResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.InfixPrecedence);
                if (maxValueResult.IsFailure)
                {
                    return maxValueResult.Map<IModifier>(_ => null!);
                }
                return Result<IModifier>.Success(new Maximum(maxValueResult.Value!));
            
            case TokenType.Exploding:
                if (!reader.TryPeek(out var nextToken) && nextToken.TokenDetails.TokenCategory != TokenCategory.Comparison)
                {
                    return Result<IModifier>.Success(new Exploding(new Max()));
                }
                
                reader.Advance();
                var comparisonResult = DiceExpressionParser.ParseExpression(ref reader, nextToken.TokenDetails.InfixPrecedence);
                
                if (comparisonResult.IsFailure)
                {
                    return comparisonResult.Map<IModifier>(_ => null!);
                }
                
                IComparison comparison = nextToken.TokenDetails.TokenType switch
                {
                    TokenType.Equal => new Equal(comparisonResult.Value!),
                    TokenType.NotEqual => new NotEqual(comparisonResult.Value!),
                    TokenType.GreaterThan => new GreaterThan(comparisonResult.Value!),
                    TokenType.GreaterThanEqual => new GreaterThanEqual(comparisonResult.Value!),
                    TokenType.LesserThan => new LesserThan(comparisonResult.Value!),
                    TokenType.LesserThanEqual => new LesserThanEqual(comparisonResult.Value!),
                    _ => new Max()
                };
                
                return Result<IModifier>.Success(new Exploding(comparison));
                
            default:
                return Result<IModifier>.Failure(
                    new ParserError("UnknownModifier", $"Unknown modifier '{token.TokenDetails.TokenType}'", reader.Position - 1));
        }
    }
}