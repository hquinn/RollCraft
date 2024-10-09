namespace LegacyRoller.Comparisons;

internal sealed class LesserThanEqual : IComparison
{
    internal LesserThanEqual(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"LESSTHANEQUAL({Comparison})";
    }
}