namespace LegacyRoller.Nodes;

internal class Divide : DiceExpression
{
    internal DiceExpression Left { get; }
    internal DiceExpression Right { get; }

    internal Divide(DiceExpression left, DiceExpression right)
    {
        Left = left;
        Right = right;
    }
    
    public override string ToString()
    {
        return $"DIVIDE({Left}, {Right})";
    }
}