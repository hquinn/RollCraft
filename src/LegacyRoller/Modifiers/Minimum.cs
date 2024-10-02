namespace LegacyRoller.Modifiers;

internal sealed class Minimum : IModifier
{
    internal Minimum(DiceExpression minimum)
    {
        MinimumValue = minimum;
    }

    public DiceExpression MinimumValue { get; }

    public int Order => 1;
    
    public override string ToString()
    {
        return $"MINIMUM={MinimumValue}";
    }
}