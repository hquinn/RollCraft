using System.Numerics;
using MonadCraft;

namespace RollCraft.Nodes;

internal sealed class Number<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal TNumber Value { get; }

    internal Number(TNumber value)
    {
        Value = value;
    }

    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Success((Value, []));
    }
    
    public override string ToString()
    {
        return Value.ToString()!;
    }
}