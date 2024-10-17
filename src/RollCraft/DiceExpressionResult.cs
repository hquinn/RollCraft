using System.Numerics;

namespace RollCraft;

public sealed class DiceExpressionResult<TNumber> where TNumber : INumber<TNumber>
{
    public DiceExpressionResult(TNumber result, List<DiceRoll>? rolls = null)
    {
        Result = result;
        Rolls = rolls ?? [];
    }
    
    public TNumber Result { get; set; }
    public List<DiceRoll> Rolls { get; set; }
}