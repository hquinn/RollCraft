using BenchmarkDotNet.Attributes;

namespace LegacyRoller.Benchmarks;

[MemoryDiagnoser]
public class EvaluatorBenchmarker
{
    private readonly DiceExpressionEvaluator _evaluator = new(new SeededRandom(0));
    private const string LongMathExpression = "1+2--3*4/5*-6-7+8*9/10--11/12*13+14*-15";
    
    [Benchmark]
    public DiceExpressionResult EvaluateLongMathExpression()
    {
        return _evaluator.Evaluate(LongMathExpression).Value!;
    }
}

public class SeededRandom : IRandom
{
    private readonly Random _random;

    public SeededRandom(int seed)
    {
        _random = new Random(seed);
    }

    public int RollDice(int dieSize)
    {
        return _random.Next(1, dieSize + 1);
    }
}