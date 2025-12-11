using System.Numerics;
using MonadCraft;

namespace RollCraft;

public abstract class DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(IRoller roller)
    {
        var result = EvaluateNode(roller);
        
        return result.IsSuccess
            ? Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Success(new DiceExpressionResult<IRollError, TNumber>(result.Value.Result, result.Value.Rolls))
            : Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(result.Error);
    }
    
    internal Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        var result = EvaluateNode(roller, variables);
        
        return result.IsSuccess
            ? Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Success(new DiceExpressionResult<IRollError, TNumber>(result.Value.Result, result.Value.Rolls))
            : Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(result.Error);
    }

    internal abstract Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller);
    
    internal virtual Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        return EvaluateNode(roller);
    }
}