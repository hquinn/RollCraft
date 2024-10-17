using System.Numerics;

namespace RollCraft.Comparisons;

internal sealed class GreaterThan<TNumber> : BaseComparison<TNumber>
    where TNumber : INumber<TNumber>
{
    internal GreaterThan(DiceExpression<TNumber> comparison) : base(comparison)
    {
    }

    protected override bool Compare(int roll, int comparisonValue)
    {
        return roll > comparisonValue;
    }

    public override string ToString()
    {
        return $"GREATERTHAN({Comparison})";
    }
}