namespace LegacyRoller.Modifiers;

internal sealed class KeepLowest : IModifier
{
    internal KeepLowest(DiceExpression countValue)
    {
        CountValue = countValue;
    }

    internal DiceExpression CountValue { get; }
    
    public override string ToString()
    {
        return $"KEEPLOWEST={CountValue}";
    }
}