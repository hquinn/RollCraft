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

    internal override Result<double> EvaluateNode(IRandom random)
    {
        return Result<double>.Success(Value);
    }
    
    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}