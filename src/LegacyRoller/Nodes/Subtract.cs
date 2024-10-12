using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Subtract : DiceExpression
{
    internal DiceExpression Left { get; }
    internal DiceExpression Right { get; }

    internal Subtract(DiceExpression left, DiceExpression right)
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

        var result = new DiceExpressionResult(leftResult.Value!.Result - rightResult.Value!.Result);
        return Result<DiceExpressionResult>.Success(result);
    }
    
    public override string ToString()
    {
        return $"SUBTRACT({Left}, {Right})";
    }
}