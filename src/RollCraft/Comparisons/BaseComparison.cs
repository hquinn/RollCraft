using System.Numerics;
using MonadCraft;

namespace RollCraft.Comparisons;

internal abstract class BaseComparison<TNumber> : IComparison
    where TNumber : INumber<TNumber>
{
    private readonly ThreadLocal<int?> _comparisonValue = new();
    private readonly ThreadLocal<List<DiceRoll>?> _rolls = new();
    
    protected BaseComparison(DiceExpression<TNumber> comparison)
    {
        Comparison = comparison;
    }
    
    internal DiceExpression<TNumber> Comparison { get; }

    public Result<IRollError, (bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll)
    {       
        if (_comparisonValue.Value is null)
        {
            var result = Comparison.Evaluate(roller);
            
            if (result.IsFailure)
            {
                return Result<IRollError, (bool Success, List<DiceRoll> Rolls)>.Failure(result.Error);
            }
            
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (!TNumber.IsInteger(result.Value.Result))
            {
                return new EvaluatorError("Evaluator.ComparisonError", "Comparison must be an integer!");
            }

            var comparisonVal = int.CreateSaturating(result.Value.Result);
            
            if (comparisonVal < 1)
            {
                return new EvaluatorError("Evaluator.ComparisonError", "Comparison must not be less than 1!");
            }
            
            if (comparisonVal > roll.Sides)
            {
                return new EvaluatorError("Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!");
            }
            
            _comparisonValue.Value = comparisonVal;
            _rolls.Value = result.Value.Rolls;
        }
        
        return Result<IRollError, (bool Success, List<DiceRoll> Rolls)>.Success((Compare(roll.Roll, _comparisonValue.Value!.Value), _rolls.Value!));
    }
    
    protected abstract bool Compare(int roll, int comparisonValue);

    public void Reset()
    {
        _comparisonValue.Value = null;
        _rolls.Value = null;
    }
}