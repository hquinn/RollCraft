namespace RollCraft.UnitTests;

/// <summary>
/// Tests for negative dice count handling.
/// </summary>
public class NegativeDiceCountTests
{
    [Test]
    public async Task Evaluate_NegativeDiceCount_ShouldNegateResult()
    {
        // -2d6 with maximum roller (each die = 6)
        // Should result in -(6 + 6) = -12
        var parseResult = DiceExpressionParser.Parse<int>("-2d6");
        await Assert.That(parseResult.IsSuccess).IsTrue();
        
        var evaluator = DiceExpressionEvaluator<int>.CreateMaximum();
        var evalResult = evaluator.Evaluate(parseResult.Value);
        
        await Assert.That(evalResult.IsSuccess).IsTrue();
        await Assert.That(evalResult.Value.Result).IsEqualTo(-12);
    }
    
    [Test]
    public async Task Evaluate_NegativeDiceCount_WithMinimumRoller_ShouldNegateResult()
    {
        // -3d6 with minimum roller (each die = 1)
        // Should result in -(1 + 1 + 1) = -3
        var parseResult = DiceExpressionParser.Parse<int>("-3d6");
        await Assert.That(parseResult.IsSuccess).IsTrue();
        
        var evaluator = DiceExpressionEvaluator<int>.CreateMinimum();
        var evalResult = evaluator.Evaluate(parseResult.Value);
        
        await Assert.That(evalResult.IsSuccess).IsTrue();
        await Assert.That(evalResult.Value.Result).IsEqualTo(-3);
    }
    
    [Test]
    public async Task Evaluate_NegativeDiceCountInExpression_ShouldNegateResult()
    {
        // 10 + -2d6 with maximum roller = 10 + (-12) = -2
        var parseResult = DiceExpressionParser.Parse<int>("10 + -2d6");
        await Assert.That(parseResult.IsSuccess).IsTrue();
        
        var evaluator = DiceExpressionEvaluator<int>.CreateMaximum();
        var evalResult = evaluator.Evaluate(parseResult.Value);
        
        await Assert.That(evalResult.IsSuccess).IsTrue();
        await Assert.That(evalResult.Value.Result).IsEqualTo(-2);
    }
    
    [Test]
    public async Task Evaluate_NegativeOneDice_ShouldNegateResult()
    {
        // -d6 with maximum roller (die = 6)
        // Should result in -6
        var parseResult = DiceExpressionParser.Parse<int>("-d6");
        await Assert.That(parseResult.IsSuccess).IsTrue();
        
        var evaluator = DiceExpressionEvaluator<int>.CreateMaximum();
        var evalResult = evaluator.Evaluate(parseResult.Value);
        
        await Assert.That(evalResult.IsSuccess).IsTrue();
        await Assert.That(evalResult.Value.Result).IsEqualTo(-6);
    }
}
