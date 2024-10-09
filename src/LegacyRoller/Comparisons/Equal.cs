namespace LegacyRoller.Comparisons;

internal sealed class Equal : IComparison
{
    public Equal(DiceExpression comparison)
    {
        Comparison = comparison;
    }

    public DiceExpression Comparison { get; }
    
    public override string ToString()
    {
        return $"EQUAL({Comparison})";
    }
}