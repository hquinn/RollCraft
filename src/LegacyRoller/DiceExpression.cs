using LitePrimitives;

namespace LegacyRoller;

public abstract class DiceExpression
{
    internal Result<DiceExpressionResult> Evaluate(IRoller roller)
    {
        var result = EvaluateNode(roller);
        
        return result.IsSuccess
            ? Result<DiceExpressionResult>.Success(new DiceExpressionResult(result.Value.Result, result.Value.Rolls))
            : result.Map<DiceExpressionResult>(_ => null!);
    }

    internal abstract Result<(double Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller);
}