using LegacyRoller.Comparisons;

namespace LegacyRoller.Modifiers;

internal sealed class Exploding : IModifier
{
    internal Exploding(IComparison comparison)
    {
        Comparison = comparison;
    }

    internal IComparison Comparison { get; }
    
    public override string ToString()
    {
        return $"EXPLODING={Comparison}";
    }
}