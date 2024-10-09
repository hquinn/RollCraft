namespace LegacyRoller.Modifiers;

internal sealed class KeepHighest : IModifier
{
    internal KeepHighest(DiceExpression countValue)
    {
        CountValue = countValue;
    }

    internal DiceExpression CountValue { get; }
    
    public override string ToString()
    {
        return $"KEEPHIGHEST={CountValue}";
    }
}