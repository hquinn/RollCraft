using System.Numerics;
using LitePrimitives;

namespace RollCraft;

public abstract class DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal Result<DiceExpressionResult<TNumber>> Evaluate(IRoller roller)
    {
        var result = EvaluateNode(roller);
        
        return result.IsSuccess
            ? Result<DiceExpressionResult<TNumber>>.Success(new DiceExpressionResult<TNumber>(result.Value.Result, result.Value.Rolls))
            : Result<DiceExpressionResult<TNumber>>.Failure(result.Error!);
    }

    internal abstract Result<(TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller);
}