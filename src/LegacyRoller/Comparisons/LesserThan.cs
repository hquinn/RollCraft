namespace LegacyRoller.Comparisons;

internal sealed class LesserThan : IComparison
{
    internal LesserThan(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"LESSTHAN({Comparison})";
    }
}