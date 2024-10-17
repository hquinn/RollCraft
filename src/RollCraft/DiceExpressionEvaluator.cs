using System.Numerics;
using LitePrimitives;
using RollCraft.Rollers;

namespace RollCraft;

public class DiceExpressionEvaluator<TNumber> 
    where TNumber : INumber<TNumber>
{
    private readonly IRoller _roller;
    
    private DiceExpressionEvaluator(IRoller roller)
    {
        _roller = roller;
    }
    
    public static DiceExpressionEvaluator<TNumber> CreateCustom(IRoller roller)
    {
        return new DiceExpressionEvaluator<TNumber>(roller);
    }

    public static DiceExpressionEvaluator<TNumber> CreateRandom()
    {
        return new DiceExpressionEvaluator<TNumber>(new RandomRoller());
    }
    
    public static DiceExpressionEvaluator<TNumber> CreateMinimum()
    {
        return new DiceExpressionEvaluator<TNumber>(new MinimumRoller());
    }

    public static DiceExpressionEvaluator<TNumber> CreateMaximum()
    {
        return new DiceExpressionEvaluator<TNumber>(new MaximumRoller());
    }

    public static DiceExpressionEvaluator<TNumber> CreateFixedAverage()
    {
        return new DiceExpressionEvaluator<TNumber>(new FixedAverageRoller());
    }

    public Result<DiceExpressionResult<TNumber>> Evaluate(DiceExpression<TNumber> expression)
    {
        return expression.Evaluate(_roller);
    }

    public Result<DiceExpressionResult<TNumber>> Evaluate(string expression)
    {
        var parsedExpression = DiceExpressionParser.Parse<TNumber>(expression);

        if (parsedExpression.IsFailure)
        {
            return Result<DiceExpressionResult<TNumber>>.Failure(parsedExpression.Errors!);
        }

        return parsedExpression.Value!.Evaluate(_roller);
    }
}