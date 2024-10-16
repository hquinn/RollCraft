using System.Globalization;
using LitePrimitives;

namespace RollCraft.Simple.Nodes;

internal sealed class Number : DiceExpression
{
    internal int Value { get; }

    internal Number(int value)
    {
        Value = value;
    }

    internal override Result<(int Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        return Result<(int Result, List<DiceRoll> Rolls)>.Success((Value, []));
    }
    
    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}