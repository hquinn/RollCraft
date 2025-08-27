using System.Numerics;

namespace RollCraft;

public sealed class DiceExpressionResult<RollError, TNumber> where TNumber : INumber<TNumber>
{
    public DiceExpressionResult(TNumber result, List<DiceRoll>? rolls = null)
    {
        Result = result;
        Rolls = rolls ?? [];
    }
    
    public TNumber Result { get; set; }
    public List<DiceRoll> Rolls { get; set; }

    public override string ToString()
    {
        return $"{Result}: [{string.Join(", ", Rolls)}]";
    }
}