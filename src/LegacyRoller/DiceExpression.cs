using LitePrimitives;

namespace LegacyRoller;

public abstract class DiceExpression
{
    public Result<DiceExpressionResult> Evaluate()
    {
        return EvaluateNode();
    }

    protected abstract Result<DiceExpressionResult> EvaluateNode();
}