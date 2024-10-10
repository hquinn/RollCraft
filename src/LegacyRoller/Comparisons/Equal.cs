namespace LegacyRoller.Comparisons;

internal sealed class Equal : IComparison
{
    internal Equal(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    internal DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"EQUAL({Comparison})";
    }
}