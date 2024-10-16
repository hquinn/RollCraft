using BenchmarkDotNet.Attributes;

namespace RollCraft.Benchmarks;

[MemoryDiagnoser]
public class EvaluatorBenchmarker
{
    private readonly Full.DiceExpressionEvaluator _fullEvaluator = Full.DiceExpressionEvaluator.CreateCustom(new FullSeededRoller(0));
    private readonly Full.DiceExpression _fullDiceExpression = Full.DiceExpressionParser.Parse("4d10min2max8!=4r=5kh2+5").Value!;
    
    
    private readonly Simple.DiceExpressionEvaluator _simpleEvaluator = Simple.DiceExpressionEvaluator.CreateCustom(new SimpleSeededRoller(0));
    private readonly Simple.DiceExpression _simpleDiceExpression = Simple.DiceExpressionParser.Parse("4d10min2max8!=4r=5kh2+5").Value!;
    
    [Benchmark]
    public Full.DiceExpressionResult Full_EvaluateDiceExpression()
    {
        return _fullEvaluator.Evaluate(_fullDiceExpression).Value!;
    }
    
    [Benchmark]
    public Simple.DiceExpressionResult Simple_EvaluateDiceExpression()
    {
        return _simpleEvaluator.Evaluate(_simpleDiceExpression).Value!;
    }
}

public class FullSeededRoller : Full.IRoller
{
    private readonly Random _random;

    public FullSeededRoller(int seed)
    {
        _random = new Random(seed);
    }

    public int RollDice(int dieSize)
    {
        return _random.Next(1, dieSize + 1);
    }
}

public class SimpleSeededRoller : Simple.IRoller
{
    private readonly Random _random;

    public SimpleSeededRoller(int seed)
    {
        _random = new Random(seed);
    }

    public int RollDice(int dieSize)
    {
        return _random.Next(1, dieSize + 1);
    }
}