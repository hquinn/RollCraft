using LitePrimitives;

namespace LegacyRoller;

public class DiceExpressionEvaluator
{
    public Result<DiceExpressionResult> Evaluate(string expression)
    {
        var parsedExpression = DiceExpressionParser.Parse(expression);

        if (parsedExpression.IsFailure)
        {
            return parsedExpression.Map<DiceExpressionResult>(_ => null!);
        }

        return parsedExpression.Value!.Evaluate();
    }
}