using LitePrimitives;

namespace RollCraft.Full.Nodes;

internal sealed class Unary : DiceExpression
{
    internal DiceExpression Expression { get; }

    internal Unary(DiceExpression expression)
    {
        Expression = expression;
    }

    internal override Result<(double Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        var result = Expression.EvaluateNode(roller);
        if (result.IsFailure)
        {
            return result;
        }

        return Result<(double Result, List<DiceRoll> Rolls)>.Success((-result.Value.Result, result.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}