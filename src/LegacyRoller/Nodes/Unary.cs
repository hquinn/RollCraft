using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Unary : DiceExpression
{
    internal DiceExpression Expression { get; }

    internal Unary(DiceExpression expression)
    {
        Expression = expression;
    }

    internal override Result<(double Result, List<DiceRoll> Rolls)> EvaluateNode(IRandom random)
    {
        var result = Expression.EvaluateNode(random);
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