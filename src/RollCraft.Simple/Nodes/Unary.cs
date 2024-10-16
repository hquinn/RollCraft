using LitePrimitives;

namespace RollCraft.Simple.Nodes;

internal sealed class Unary : DiceExpression
{
    internal DiceExpression Expression { get; }

    internal Unary(DiceExpression expression)
    {
        Expression = expression;
    }

    internal override Result<(int Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        var result = Expression.EvaluateNode(roller);
        if (result.IsFailure)
        {
            return result;
        }

        return Result<(int Result, List<DiceRoll> Rolls)>.Success((-result.Value.Result, result.Value.Rolls));
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}