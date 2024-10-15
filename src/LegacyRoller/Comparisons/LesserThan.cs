using LitePrimitives;

namespace LegacyRoller.Comparisons;

internal sealed class LesserThan : IComparison
{
    private long? _comparisonValue;
    
    internal LesserThan(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }

    public Result<bool> RollEquals(IRandom random, DiceRoll roll)
    {
        if (_comparisonValue is null)
        {
            var result = Comparison.Evaluate(random);
            
            if (result.IsFailure)
            {
                return result.Map<bool>(_ => default);
            }
            
            var comparison = (long) result.Value!.Result;
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (result.Value!.Result != comparison)
            {
                return Result<bool>.Failure(new EvaluatorError("ComparisonError", "Comparison must be an integer!"));
            }
            
            if (comparison < 1)
            {
                return Result<bool>.Failure(new EvaluatorError("ComparisonError", "Comparison must not be less than 1!"));
            }
            
            if (comparison > roll.Sides)
            {
                return Result<bool>.Failure(new EvaluatorError("ComparisonError", "Comparison must not be greater than the dice side count!"));
            }
            
            _comparisonValue = comparison;
        }
        
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return Result<bool>.Success(roll.Roll < _comparisonValue);
    }

    public void Reset()
    {
        _comparisonValue = null;
    }
    
    public override string ToString()
    {
        return $"LESSTHAN({Comparison})";
    }
}