namespace LegacyRoller.Comparisons;

internal sealed class GreaterThan : IComparison
{
    internal GreaterThan(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"GREATERTHAN({Comparison})";
    }
}