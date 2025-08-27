using BenchmarkDotNet.Attributes;

namespace RollCraft.Benchmarks;

[MemoryDiagnoser]
public class Benchmarker
{
    private const string DiceExpression1 = "1d6+3";
    private const string DiceExpression2 = "4d6kh3";
    private const string DiceExpression3 = "4d10min2max8!=4r=5kh2+5";
    private const string DiceExpression4 = "1+2*3-4+5*6-7+8*9-10+11*12-13+14*15-16";
    private const string DiceExpression5 = "(1d6)d(1d10)min4!=(1d8)k(1d4)ro=10";
    
    private readonly DiceExpressionEvaluator<double> _fullEvaluator = DiceExpressionEvaluator<double>.CreateMaximum();
    private readonly DiceExpressionEvaluator<int> _simpleEvaluator = DiceExpressionEvaluator<int>.CreateMaximum();

    public IEnumerable<string> Expressions =>
    [
        DiceExpression1,
        DiceExpression2,
        DiceExpression3,
        DiceExpression4,
        DiceExpression5,
    ];

    public IEnumerable<DiceExpression<double>> FullDiceExpressions
    {
        get
        {
            foreach (var expression in Expressions)
            {
                yield return DiceExpressionParser.Parse<double>(expression).Value;
            }
        }
    }
    
    public IEnumerable<DiceExpression<int>> SimpleDiceExpressions
    {
        get
        {
            foreach (var expression in Expressions)
            {
                yield return DiceExpressionParser.Parse<int>(expression).Value;
            }
        }
    }
                        
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public DiceExpression<double> Full_Parse(string expression)
    {
        return DiceExpressionParser.Parse<double>(expression).Value;
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(Expressions))]
    public DiceExpression<int> Simple_Parse(string expression)
    {
        return DiceExpressionParser.Parse<int>(expression).Value;
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(FullDiceExpressions))]
    public DiceExpressionResult<IRollError, double> Full_Evaluate(DiceExpression<double> expression)
    {
        return _fullEvaluator.Evaluate(expression).Value;
    }
    
    [Benchmark]
    [ArgumentsSource(nameof(SimpleDiceExpressions))]
    public DiceExpressionResult<IRollError, int> Simple_Evaluate(DiceExpression<int> expression)
    {
        return _simpleEvaluator.Evaluate(expression).Value;
    }
}