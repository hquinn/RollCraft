namespace LegacyRoller.Modifiers;

internal sealed class Maximum : IModifier
{
    internal Maximum(DiceExpression maximum)
    {
        MaximumValue = maximum;
    }

    public DiceExpression MaximumValue { get; }

    public int Order => 1;
    
    public override string ToString()
    {
        return $"MAXIMUM={MaximumValue}";
    }
}