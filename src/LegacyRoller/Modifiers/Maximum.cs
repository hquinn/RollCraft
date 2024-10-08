namespace LegacyRoller.Modifiers;

internal sealed class Maximum : IModifier
{
    internal Maximum(DiceExpression maximum)
    {
        MaximumValue = maximum;
    }

    internal DiceExpression MaximumValue { get; }
    
    public override string ToString()
    {
        return $"MAXIMUM={MaximumValue}";
    }
}