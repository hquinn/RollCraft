using System.Globalization;
using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Number : DiceExpression
{
    internal double Value { get; }

    internal Number(double value)
    {
        Value = value;
    }

    protected override Result<DiceExpressionResult> EvaluateNode()
    {
        return Result<DiceExpressionResult>.Success(new DiceExpressionResult(Value));
    }
    
    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}