namespace LegacyRoller.Nodes;

internal class Subtract : DiceExpression
{
    internal DiceExpression Left { get; }
    internal DiceExpression Right { get; }

    internal Subtract(DiceExpression left, DiceExpression right)
    {
        Left = left;
        Right = right;
    }
    
    public override string ToString()
    {
        return $"SUBTRACT({Left}, {Right})";
    }
}