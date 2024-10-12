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

    internal override Result<double> EvaluateNode(IRandom random)
    {
        var leftResult = Left.EvaluateNode(random);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        var rightResult = Right.EvaluateNode(random);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        return Result<double>.Success(leftResult.Value - rightResult.Value);
    }
    
    public override string ToString()
    {
        return $"SUBTRACT({Left}, {Right})";
    }
}