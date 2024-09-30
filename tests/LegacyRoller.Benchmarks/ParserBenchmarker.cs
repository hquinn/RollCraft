using BenchmarkDotNet.Attributes;

namespace LegacyRoller.Benchmarks;

[MemoryDiagnoser]
public class ParserBenchmarker
{
    private const string LongMathExpression = "1+2--3*4/5*-6-7+8*9/10--11/12*13+14*-15";
    
    [Benchmark]
    public DiceExpression ParseLongMathExpression()
    {
        return DiceExpressionParser.Parse(LongMathExpression).Value!;
    }
}