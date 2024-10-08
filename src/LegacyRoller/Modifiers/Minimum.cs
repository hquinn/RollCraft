namespace LegacyRoller.Modifiers;

internal sealed class Minimum : IModifier
{
    internal Minimum(DiceExpression minimum)
    {
        MinimumValue = minimum;
    }

    internal DiceExpression MinimumValue { get; }
    
    public override string ToString()
    {
        return $"MINIMUM={MinimumValue}";
    }
}