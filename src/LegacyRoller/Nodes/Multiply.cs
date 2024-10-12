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

    protected override Result<DiceExpressionResult> EvaluateNode(IRandom random)
    {        
        var leftResult = Left.Evaluate(random);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        var rightResult = Right.Evaluate(random);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        leftResult.Value!.Result *= rightResult.Value!.Result;
        return leftResult;
    }
    
    public override string ToString()
    {
        return $"MULTIPLY({Left}, {Right})";
    }
}