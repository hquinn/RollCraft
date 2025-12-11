using System.Numerics;
using MonadCraft;

namespace RollCraft.Nodes;

internal sealed class Variable<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal string Name { get; }

    internal Variable(string name)
    {
        Name = name;
    }

    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        return new EvaluatorError(
            "Evaluator.UnresolvedVariable",
            $"Variable '{Name}' was not resolved before evaluation!");
    }
    
    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        if (variables.TryGetValue(Name, out var value))
        {
            return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Success((value, []));
        }
        
        return new EvaluatorError(
            "Evaluator.UndefinedVariable",
            $"Variable '{Name}' is not defined!");
    }
    
    public override string ToString()
    {
        return $"[{Name}]";
    }
}
