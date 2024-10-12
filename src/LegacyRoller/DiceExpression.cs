using LitePrimitives;

namespace LegacyRoller;

public abstract class DiceExpression
{
    internal Result<DiceExpressionResult> Evaluate(IRandom random)
    {
        return EvaluateNode(random);
    }

    protected abstract Result<DiceExpressionResult> EvaluateNode(IRandom random);
}