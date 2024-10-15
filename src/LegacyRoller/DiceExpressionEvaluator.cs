using LegacyRoller.Randomizer;
using LitePrimitives;

namespace LegacyRoller;

public class DiceExpressionEvaluator
{
    private readonly IRandom _random;

    public DiceExpressionEvaluator()
        : this(new DefaultRandom())
    {
    }
    
    public DiceExpressionEvaluator(IRandom random)
    {
        _random = random;
    }

    public Result<DiceExpressionResult> Evaluate(DiceExpression expression)
    {
        return expression.Evaluate(_random);
    }

    public Result<DiceExpressionResult> Evaluate(string expression)
    {
        var parsedExpression = DiceExpressionParser.Parse(expression);

        if (parsedExpression.IsFailure)
        {
            return parsedExpression.Map<DiceExpressionResult>(_ => null!);
        }

        return parsedExpression.Value!.Evaluate(_random);
    }
}