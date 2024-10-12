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
        throw new NotImplementedException();
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}