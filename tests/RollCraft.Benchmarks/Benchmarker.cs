using BenchmarkDotNet.Attributes;

namespace RollCraft.Benchmarks;

[MemoryDiagnoser]
public class Benchmarker
{
    private const string DiceExpression = "4d10min2max8!=4r=5kh2+5";
    
    private readonly DiceExpressionEvaluator<double> _fullEvaluator = DiceExpressionEvaluator<double>.CreateMaximum();
    private readonly DiceExpression<double> _fullDiceExpression = DiceExpressionParser.Parse<double>(DiceExpression).Value!;
    
    private readonly DiceExpressionEvaluator<int> _simpleEvaluator = DiceExpressionEvaluator<int>.CreateMaximum();
    private readonly DiceExpression<int> _simpleDiceExpression = DiceExpressionParser.Parse<int>(DiceExpression).Value!;
    
    [Benchmark]
    public DiceExpression<double> Full_ParseDiceExpression()
    {
        return DiceExpressionParser.Parse<double>(DiceExpression).Value!;
    }
    
    [Benchmark]
    public DiceExpression<int> Simple_ParseDiceExpression()
    {
        return DiceExpressionParser.Parse<int>(DiceExpression).Value!;
    }
    
    [Benchmark]
    public DiceExpressionResult<int> Simple_EvaluateDiceExpression()
    {
        return _simpleEvaluator.Evaluate(_simpleDiceExpression).Value!;
    }
    
    [Benchmark]
    public DiceExpressionResult<double> Full_EvaluateDiceExpression()
    {
        return _fullEvaluator.Evaluate(_fullDiceExpression).Value!;
    }
}