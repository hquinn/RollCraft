namespace RollCraft.UnitTests;

/// <summary>
/// Tests for input validation in DiceExpressionParser.
/// </summary>
public class InputValidationTests
{
    [Test]
    public async Task Parse_NullInput_ShouldReturnError()
    {
        var result = DiceExpressionParser.Parse<int>(null!);
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("NULL_INPUT");
    }
    
    [Test]
    public async Task Parse_EmptyInput_ShouldReturnError()
    {
        var result = DiceExpressionParser.Parse<int>("");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("EMPTY_INPUT");
    }
    
    [Test]
    public async Task Parse_WhitespaceOnlyInput_ShouldReturnError()
    {
        var result = DiceExpressionParser.Parse<int>("   ");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error.ErrorCode).IsEqualTo("EMPTY_INPUT");
    }
    
    [Test]
    public async Task TryParse_NullInput_ShouldReturnError()
    {
        var error = DiceExpressionParser.TryParse<int>(null!, out var expression);
        
        await Assert.That(error).IsNotNull();
        await Assert.That(expression).IsNull();
        await Assert.That(error!.Value.ErrorCode).IsEqualTo("NULL_INPUT");
    }
    
    [Test]
    public async Task TryParse_EmptyInput_ShouldReturnError()
    {
        var error = DiceExpressionParser.TryParse<int>("", out var expression);
        
        await Assert.That(error).IsNotNull();
        await Assert.That(expression).IsNull();
        await Assert.That(error!.Value.ErrorCode).IsEqualTo("EMPTY_INPUT");
    }
}
