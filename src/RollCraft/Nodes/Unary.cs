using System.Numerics;
using LitePrimitives;

namespace RollCraft.Nodes;

internal sealed class Unary<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal DiceExpression<TNumber> Expression { get; }

    internal Unary(DiceExpression<TNumber> expression)
    {
        Expression = expression;
    }

    internal override Result<(TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        var result = Expression.EvaluateNode(roller);
        if (result.IsFailure)
        {
            return result;
        }

        return Result<(TNumber Result, List<DiceRoll> Rolls)>.Success((-result.Value.Result, result.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}