using LitePrimitives;

namespace LegacyRoller;

public abstract class DiceExpression
{
    internal Result<DiceExpressionResult> Evaluate(IRandom random)
    {
        var result = EvaluateNode(random);
        
        return result.IsSuccess
            ? Result<DiceExpressionResult>.Success(new DiceExpressionResult(result.Value.Result, result.Value.Rolls))
            : result.Map<DiceExpressionResult>(_ => null!);
    }

    internal abstract Result<(double Result, List<DiceRoll> Rolls)> EvaluateNode(IRandom random);
}