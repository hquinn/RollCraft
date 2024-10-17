using System.Numerics;

namespace RollCraft.Comparisons;

internal sealed class GreaterThanEqual<TNumber> : BaseComparison<TNumber>
    where TNumber : INumber<TNumber>
{
    internal GreaterThanEqual(DiceExpression<TNumber> comparison) : base(comparison)
    {
    }

    protected override bool Compare(int roll, int comparisonValue)
    {
        return roll >= comparisonValue;
    }
    
    public override string ToString()
    {
        return $"GREATERTHANEQUAL({Comparison})";
    }
}