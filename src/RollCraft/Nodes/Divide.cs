using System.Numerics;
using LitePrimitives;

namespace RollCraft.Nodes;

internal sealed class Divide<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal DiceExpression<TNumber> Left { get; }
    internal DiceExpression<TNumber> Right { get; }

    internal Divide(DiceExpression<TNumber> left, DiceExpression<TNumber> right)
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
        
        if (TNumber.IsZero(rightResult.Value.Result))
        {
            return Error.Default("Evaluator.DivideByZero", "Division by zero detected!");
        }
        
        var result = leftResult.Value.Result / rightResult.Value.Result;
        leftResult.Value.Rolls.AddRange(rightResult.Value.Rolls);
        
        return Result<(TNumber Result, List<DiceRoll> Rolls)>.Success((result, leftResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"DIVIDE({Left}, {Right})";
    }
}