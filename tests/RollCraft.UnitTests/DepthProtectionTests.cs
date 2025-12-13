namespace RollCraft.UnitTests;

/// <summary>
/// Tests for depth protection in deeply nested expressions.
/// </summary>
public class DepthProtectionTests
{
    [Test]
    public async Task Parse_DeeplyNestedParentheses_ShouldReturnError()
    {
        // Create an expression with 300 nested parentheses: ((((....1))))
        // This exceeds the max depth of 256
        var depth = 300;
        var expression = new string('(', depth) + "1" + new string(')', depth);
        
        var result = DiceExpressionParser.Parse<int>(expression);
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("MAX_DEPTH_EXCEEDED");
    }
    
    [Test]
    public async Task Parse_DeeplyNestedUnary_ShouldReturnError()
    {
        // Create an expression with 300 unary minuses: ------...1
        var depth = 300;
        var expression = new string('-', depth) + "1";
        
        var result = DiceExpressionParser.Parse<int>(expression);
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("MAX_DEPTH_EXCEEDED");
    }
    
    [Test]
    public async Task Parse_DeeplyNestedFunctions_ShouldReturnError()
    {
        // Create nested floor calls: floor(floor(floor(...1)))
        var depth = 300;
        var expression = string.Concat(Enumerable.Repeat("floor(", depth)) + "1" + new string(')', depth);
        
        var result = DiceExpressionParser.Parse<int>(expression);
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("MAX_DEPTH_EXCEEDED");
    }
    
    [Test]
    public async Task Parse_NormalDepth_ShouldSucceed()
    {
        // A reasonably nested expression should work fine
        var expression = "((1 + 2) * (3 + 4)) + floor(ceil(5.5))";
        
        var result = DiceExpressionParser.Parse<double>(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
    }
    
    [Test]
    public async Task Parse_ModerateNesting_ShouldSucceed()
    {
        // 50 levels of nesting should still work
        var depth = 50;
        var expression = new string('(', depth) + "1" + new string(')', depth);
        
        var result = DiceExpressionParser.Parse<int>(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
    }
    
    [Test]
    public async Task Parse_AtMaxDepth_ShouldSucceed()
    {
        // Exactly at the limit (256) should still work
        var depth = 255; // 255 because we start at 0
        var expression = new string('(', depth) + "1" + new string(')', depth);
        
        var result = DiceExpressionParser.Parse<int>(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
    }
}
