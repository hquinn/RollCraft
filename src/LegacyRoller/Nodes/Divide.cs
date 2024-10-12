using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Divide : DiceExpression
{
    internal DiceExpression Left { get; }
    internal DiceExpression Right { get; }

    internal Divide(DiceExpression left, DiceExpression right)
    {
        Left = left;
        Right = right;
    }

    protected override Result<DiceExpressionResult> EvaluateNode()
    {
        throw new NotImplementedException();
    }
    
    public override string ToString()
    {
        return $"DIVIDE({Left}, {Right})";
    }
}