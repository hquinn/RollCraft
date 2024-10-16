using LitePrimitives;

namespace RollCraft.Simple.Comparisons;

internal sealed class LesserThanEqual : IComparison
{
    private long? _comparisonValue;
    private List<DiceRoll>? _rolls;
    
    internal LesserThanEqual(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }

    public Result<(bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll)
    {
        if (_comparisonValue is null)
        {
            var result = Comparison.Evaluate(roller);
            
            if (result.IsFailure)
            {
                return result.Map<(bool Success, List<DiceRoll> Rolls)>(_ => default);
            }

            var comparison = result.Value!.Result;
            if (comparison < 1)
            {
                return Result<(bool Success, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("ComparisonError", "Comparison must not be less than 1!"));
            }
            
            if (comparison > roll.Sides)
            {
                return Result<(bool Success, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("ComparisonError", "Comparison must not be greater than the dice side count!"));
            }
            
            _comparisonValue = comparison;
            _rolls = result.Value!.Rolls;
        }
        
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return Result<(bool Success, List<DiceRoll> Rolls)>.Success((roll.Roll <= _comparisonValue, _rolls!));
    }

    public void Reset()
    {
        _comparisonValue = null;
        _rolls = null;
    }
    
    public override string ToString()
    {
        return $"LESSTHANEQUAL({Comparison})";
    }
}