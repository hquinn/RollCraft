namespace LegacyRoller.Comparisons;

internal sealed class GreaterThanEqual : IComparison
{
    internal GreaterThanEqual(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"GREATERTHANEQUAL({Comparison})";
    }
}