using LegacyRoller.Comparisons;

namespace LegacyRoller.Modifiers;

internal sealed class ReRoll : IModifier
{
    internal ReRoll(IComparison comparison, bool reRollOnce)
    {
        Comparison = comparison;
        ReRollOnce = reRollOnce;
    }

    internal IComparison Comparison { get; }
    internal bool ReRollOnce { get; }

    public override string ToString()
    {
        return $"REROLL{(ReRollOnce ? "ONCE" : "")}={Comparison}";
    }
}