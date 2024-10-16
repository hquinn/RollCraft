using BenchmarkDotNet.Attributes;

namespace RollCraft.Benchmarks;

[MemoryDiagnoser]
public class ParserBenchmarker
{
    private const string DiceExpression = "4d10min2max8!=4r=5kh2+5";
    
    [Benchmark]
    public Full.DiceExpression Full_ParseDiceExpression()
    {
        return Full.DiceExpressionParser.Parse(DiceExpression).Value!;
    }
    
    [Benchmark]
    public Simple.DiceExpression Simple_ParseDiceExpression()
    {
        return Simple.DiceExpressionParser.Parse(DiceExpression).Value!;
    }
}