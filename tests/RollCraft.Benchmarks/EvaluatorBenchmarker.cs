using BenchmarkDotNet.Attributes;
using RollCraft.Full;

namespace RollCraft.Benchmarks;

[MemoryDiagnoser]
public class EvaluatorBenchmarker
{
    private readonly DiceExpressionEvaluator _evaluator = DiceExpressionEvaluator.CreateCustom(new SeededRoller(0));
    private readonly DiceExpression LongMathExpression = DiceExpressionParser.Parse("1+2--3*4/5*-6-7+8*9/10--11/12*13+14*-15").Value!;
    private readonly DiceExpression DiceExpression = DiceExpressionParser.Parse("4d10min2max8!=4r=5kh2+5").Value!;
    
    [Benchmark]
    public DiceExpressionResult EvaluateLongMathExpression()
    {
        return _evaluator.Evaluate(LongMathExpression).Value!;
    }
    
    [Benchmark]
    public DiceExpressionResult EvaluateDiceExpression()
    {
        return _evaluator.Evaluate(DiceExpression).Value!;
    }
}

public class SeededRoller : IRoller
{
    private readonly Random _random;

    public SeededRoller(int seed)
    {
        _random = new Random(seed);
    }

    public int RollDice(int dieSize)
    {
        return _random.Next(1, dieSize + 1);
    }
}