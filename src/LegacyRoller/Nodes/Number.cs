namespace LegacyRoller.Nodes;

internal sealed class Number : DiceExpression
{
    internal double Value { get; }

    internal Number(double value)
    {
        Value = value;
    }
    
    public override string ToString()
    {
        return $"NUMBER({Value})";
    }
}