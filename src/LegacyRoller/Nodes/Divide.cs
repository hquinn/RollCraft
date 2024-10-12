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
        
        if (rightResult.Value == 0)
        {
            return Result<double>.Failure(new EvaluatorError("DivideByZero", "Division by zero detected!"));
        }

        
        return Result<double>.Success(leftResult.Value / rightResult.Value);
    }
    
    public override string ToString()
    {
        return $"DIVIDE({Left}, {Right})";
    }
}