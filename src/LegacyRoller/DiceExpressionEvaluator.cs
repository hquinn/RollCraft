using LegacyRoller.Rollers;
using LitePrimitives;

namespace LegacyRoller;

public class DiceExpressionEvaluator
{
    private readonly IRoller _roller;
    
    private DiceExpressionEvaluator(IRoller roller)
    {
        _roller = roller;
    }
    
    public static DiceExpressionEvaluator CreateCustom(IRoller roller)
    {
        return new DiceExpressionEvaluator(roller);
    }

    public static DiceExpressionEvaluator CreateRandom()
    {
        return new DiceExpressionEvaluator(new RandomRoller());
    }
    
    public static DiceExpressionEvaluator CreateMinimum()
    {
        return new DiceExpressionEvaluator(new MinimumRoller());
    }

    public static DiceExpressionEvaluator CreateMaximum()
    {
        return new DiceExpressionEvaluator(new MaximumRoller());
    }

    public static DiceExpressionEvaluator CreateFixedAverage()
    {
        return new DiceExpressionEvaluator(new FixedAverageRoller());
    }

    public Result<DiceExpressionResult> Evaluate(DiceExpression expression)
    {
        return expression.Evaluate(_roller);
    }

    public Result<DiceExpressionResult> Evaluate(string expression)
    {
        var parsedExpression = DiceExpressionParser.Parse(expression);

        if (parsedExpression.IsFailure)
        {
            return parsedExpression.Map<DiceExpressionResult>(_ => null!);
        }

        return parsedExpression.Value!.Evaluate(_roller);
    }
}