namespace RollCraft.UnitTests;

/// <summary>
/// Tests for overflow handling in numeric lexers.
/// </summary>
public class LexerOverflowTests
{
    // Short: max value is 32767
    [Test]
    public async Task Parse_ShortOverflow_ShouldReturnError()
    {
        // 32768 overflows short (max 32767)
        var result = DiceExpressionParser.Parse<short>("32768");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    [Test]
    public async Task Parse_ShortMaxValue_ShouldSucceed()
    {
        // 32767 is valid for short
        var result = DiceExpressionParser.Parse<short>("32767");
        
        await Assert.That(result.IsSuccess).IsTrue();
    }
    
    [Test]
    public async Task Parse_ShortLargeOverflow_ShouldReturnError()
    {
        // Very large number should overflow
        var result = DiceExpressionParser.Parse<short>("999999999");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    // Int: max value is 2147483647
    [Test]
    public async Task Parse_IntOverflow_ShouldReturnError()
    {
        // 2147483648 overflows int (max 2147483647)
        var result = DiceExpressionParser.Parse<int>("2147483648");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    [Test]
    public async Task Parse_IntMaxValue_ShouldSucceed()
    {
        // 2147483647 is valid for int
        var result = DiceExpressionParser.Parse<int>("2147483647");
        
        await Assert.That(result.IsSuccess).IsTrue();
    }
    
    [Test]
    public async Task Parse_IntLargeOverflow_ShouldReturnError()
    {
        // Very large number should overflow
        var result = DiceExpressionParser.Parse<int>("99999999999999999999");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    // Long: max value is 9223372036854775807
    [Test]
    public async Task Parse_LongOverflow_ShouldReturnError()
    {
        // 9223372036854775808 overflows long (max 9223372036854775807)
        var result = DiceExpressionParser.Parse<long>("9223372036854775808");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    [Test]
    public async Task Parse_LongMaxValue_ShouldSucceed()
    {
        // 9223372036854775807 is valid for long
        var result = DiceExpressionParser.Parse<long>("9223372036854775807");
        
        await Assert.That(result.IsSuccess).IsTrue();
    }
    
    [Test]
    public async Task Parse_LongLargeOverflow_ShouldReturnError()
    {
        // Very large number should overflow
        var result = DiceExpressionParser.Parse<long>("99999999999999999999999999999999");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    // Test overflow in expressions
    [Test]
    public async Task Parse_IntOverflowInExpression_ShouldReturnError()
    {
        // Overflow in middle of expression
        var result = DiceExpressionParser.Parse<int>("1 + 2147483648 + 2");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
    
    [Test]
    public async Task Parse_ShortOverflowInDiceExpression_ShouldReturnError()
    {
        // Overflow in dice sides
        var result = DiceExpressionParser.Parse<short>("1d40000");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("OVERFLOW");
    }
}
