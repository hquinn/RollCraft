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
        
        if (rightResult.Value!.Result == 0)
        {
            return Result<DiceExpressionResult>.Failure(new EvaluatorError("DivideByZero", "Division by zero detected!"));
        }

        
        leftResult.Value!.Result /= rightResult.Value!.Result;
        return leftResult;
    }
    
    public override string ToString()
    {
        return $"DIVIDE({Left}, {Right})";
    }
}