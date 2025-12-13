namespace RollCraft.UnitTests;

/// <summary>
/// Tests for thread safety of SeededRandomRoller.
/// </summary>
public class ThreadSafetyTests
{
    [Test]
    public async Task SeededRandomRoller_ConcurrentAccess_ShouldBeThreadSafe()
    {
        // Parse a simple dice expression once
        var parseResult = DiceExpressionParser.Parse<int>("10d6");
        await Assert.That(parseResult.IsSuccess).IsTrue();
        
        // Create a single evaluator with a seeded roller
        var evaluator = DiceExpressionEvaluator<int>.CreateSeededRandom(42);
        
        // Run concurrent evaluations - 10 threads, 10 iterations each
        var failed = false;
        
        await Parallel.ForAsync(0, 100, async (_, _) =>
        {
            var result = evaluator.Evaluate(parseResult.Value);
            
            // If not thread-safe, Random can return values outside expected range
            // 10d6: min = 10, max = 60
            if (result.IsFailure || result.Value.Result < 10 || result.Value.Result > 60)
            {
                failed = true;
            }
            
            await Task.CompletedTask;
        });
        
        await Assert.That(failed).IsFalse();
    }
    
    [Test]
    public async Task SeededRandomRoller_SameSeeds_DifferentInstances_ShouldProduceSameResults()
    {
        // Two evaluators with same seed should produce same results when used independently
        var parseResult = DiceExpressionParser.Parse<int>("5d10");
        await Assert.That(parseResult.IsSuccess).IsTrue();
        
        var evaluator1 = DiceExpressionEvaluator<int>.CreateSeededRandom(12345);
        var evaluator2 = DiceExpressionEvaluator<int>.CreateSeededRandom(12345);
        
        var result1 = evaluator1.Evaluate(parseResult.Value);
        var result2 = evaluator2.Evaluate(parseResult.Value);
        
        await Assert.That(result1.IsSuccess).IsTrue();
        await Assert.That(result2.IsSuccess).IsTrue();
        await Assert.That(result1.Value.Result).IsEqualTo(result2.Value.Result);
    }
}
