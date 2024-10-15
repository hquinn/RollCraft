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

    internal override Result<(double Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {        
        var leftResult = Left.EvaluateNode(roller);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        var rightResult = Right.EvaluateNode(roller);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }
        
        if (rightResult.Value.Result == 0)
        {
            return Result<(double Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DivideByZero", "Division by zero detected!"));
        }
        
        var result = leftResult.Value.Result / rightResult.Value.Result;
        leftResult.Value.Rolls.AddRange(rightResult.Value.Rolls);
        
        return Result<(double Result, List<DiceRoll> Rolls)>.Success((result, leftResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"DIVIDE({Left}, {Right})";
    }
}