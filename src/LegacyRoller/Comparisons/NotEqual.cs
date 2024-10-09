namespace LegacyRoller.Comparisons;

internal sealed class NotEqual : IComparison
{
    internal NotEqual(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"NOTEQUAL({Comparison})";
    }
}