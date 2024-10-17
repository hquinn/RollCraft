using System.Numerics;
using LitePrimitives;

namespace RollCraft.Nodes;

internal sealed class Subtract<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal DiceExpression<TNumber> Left { get; }
    internal DiceExpression<TNumber> Right { get; }

    internal Subtract(DiceExpression<TNumber> left, DiceExpression<TNumber> right)
    {
        Left = left;
        Right = right;
    }

    internal override Result<(TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
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
        
        return Result<(TNumber Result, List<DiceRoll> Rolls)>.Success((result, leftResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"SUBTRACT({Left}, {Right})";
    }
}