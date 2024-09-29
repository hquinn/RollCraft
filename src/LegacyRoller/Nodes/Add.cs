namespace LegacyRoller.Nodes;

internal sealed class Add : DiceExpression
{
    internal DiceExpression Left { get; }
    internal DiceExpression Right { get; }

    internal Add(DiceExpression left, DiceExpression right)
    {
        Left = left;
        Right = right;
    }
    
    public override string ToString()
    {
        return $"ADD({Left}, {Right})";
    }
}