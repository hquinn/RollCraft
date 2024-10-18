using System.Numerics;
using LitePrimitives;

namespace RollCraft.Comparisons;

internal abstract class BaseComparison<TNumber> : IComparison
    where TNumber : INumber<TNumber>
{
    protected int? _comparisonValue;
    protected List<DiceRoll>? _rolls;
    
    protected BaseComparison(DiceExpression<TNumber> comparison)
    {
        Comparison = comparison;
    }
    
    internal DiceExpression<TNumber> Comparison { get; }

    public Result<(bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll)
    {       
        if (_comparisonValue is null)
        {
            var result = Comparison.Evaluate(roller);
            
            if (result.IsFailure)
            {
                return Result<(bool Success, List<DiceRoll> Rolls)>.Failure(result.Error!);
            }
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!TNumber.IsInteger(result.Value!.Result))
            {
                return Result<(bool Success, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("ComparisonError", "Comparison must be an integer!"));
            }

            var comparison = int.CreateSaturating(result.Value!.Result);
            
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
        
        return Result<(bool Success, List<DiceRoll> Rolls)>.Success((Compare(roll.Roll, _comparisonValue.Value), _rolls!));
    }
    
    protected abstract bool Compare(int roll, int comparisonValue);

    public void Reset()
    {
        _comparisonValue = null;
        _rolls = null;
    }
}