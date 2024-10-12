using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Multiply : DiceExpression
{
    internal DiceExpression Left { get; }
    internal DiceExpression Right { get; }

    internal Multiply(DiceExpression left, DiceExpression right)
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
        return $"MULTIPLY({Left}, {Right})";
    }
}