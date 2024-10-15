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
        
        var result = leftResult.Value.Result - rightResult.Value.Result;
        leftResult.Value.Rolls.AddRange(rightResult.Value.Rolls);
        
        return Result<(double Result, List<DiceRoll> Rolls)>.Success((result, leftResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"SUBTRACT({Left}, {Right})";
    }
}