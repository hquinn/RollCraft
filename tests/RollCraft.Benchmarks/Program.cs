using BenchmarkDotNet.Running;
using RollCraft.Benchmarks;

BenchmarkRunner.Run<Benchmarker>();

// const string DiceExpression = "4d10min2max8!=4r=5kh2+5";
//
// for (var i = 0; i < 1000000; i++)
// {
//     DiceExpressionParser.Parse<int>(DiceExpression);
// }