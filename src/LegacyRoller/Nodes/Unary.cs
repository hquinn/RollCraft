using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Unary : DiceExpression
{
    internal DiceExpression Expression { get; }

    internal Unary(DiceExpression expression)
    {
        Expression = expression;
    }

    protected override Result<DiceExpressionResult> EvaluateNode()
    {
        var result = Expression.Evaluate();
        if (result.IsFailure)
        {
            return result;
        }

        result.Value!.Result = -result.Value!.Result;
        return result;
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}