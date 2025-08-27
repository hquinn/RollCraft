using System.Numerics;
using MonadCraft;
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

    public static DiceExpressionEvaluator<TNumber> CreateSeededRandom(int seed)
    {
        return new DiceExpressionEvaluator<TNumber>(new SeededRandomRoller(seed));
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

    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(Result<IRollError, DiceExpression<TNumber>> expression)
    {
        if (expression.IsFailure)
        {
            return Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(expression.Error);
        }
        
        return Evaluate(expression.Value);
    }

    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(DiceExpression<TNumber> expression)
    {
        return expression.Evaluate(_roller);
    }

    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(string expression)
    {
        var parsedExpression = DiceExpressionParser.Parse<TNumber>(expression);

        if (parsedExpression.IsFailure)
        {
            return Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(parsedExpression.Error);
        }

        return parsedExpression.Value.Evaluate(_roller);
    }

    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[] Evaluate(DiceExpression<TNumber> expression, int repeatCount)
    {
        var results = new Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[repeatCount];
        
        for (var i = 0; i < repeatCount; i++)
        {
            results[i] = Evaluate(expression);
        }
        
        return results;
    }

    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[] Evaluate(string expression, int repeatCount)
    {
        var results = new Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[repeatCount];

        for (var i = 0; i < repeatCount; i++)
        {
            results[i] =  Evaluate(expression);
        }

        return results;
    }
}