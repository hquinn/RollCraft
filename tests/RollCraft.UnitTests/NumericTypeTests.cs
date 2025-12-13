using RollCraft.UnitTests.Helpers;

namespace RollCraft.UnitTests;

public class NumericTypeTests
{
    // Short type tests
    [Test]
    [Arguments("1", (short)1)]
    [Arguments("2", (short)2)]
    [Arguments("10", (short)10)]
    [Arguments("-1", (short)-1)]
    [Arguments("2+3", (short)5)]
    [Arguments("10-4", (short)6)]
    [Arguments("3*4", (short)12)]
    [Arguments("10/3", (short)3)] // Integer division
    [Arguments("10%3", (short)1)]
    [Arguments("(2+3)*4", (short)20)]
    public async Task Evaluate_Short_Expression_Should_Return_Correct_Result(string expression, short expected)
    {
        var sut = DiceExpressionEvaluator<short>.CreateMinimum();
        
        var result = sut.Evaluate(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(expected);
    }
    
    [Test]
    public async Task Evaluate_Short_DiceRoll_Should_Return_Minimum()
    {
        var sut = DiceExpressionEvaluator<short>.CreateMinimum();
        
        var result = sut.Evaluate("2d6");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo((short)2);
    }
    
    [Test]
    public async Task Evaluate_Short_With_Variables_Should_Work()
    {
        var sut = DiceExpressionEvaluator<short>.CreateMinimum();
        var variables = new Dictionary<string, short> { ["STR"] = 5 };
        
        var result = sut.Evaluate("1d20+[STR]", variables);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo((short)6);
    }
    
    // Long type tests
    [Test]
    [Arguments("1", 1L)]
    [Arguments("2", 2L)]
    [Arguments("10", 10L)]
    [Arguments("-1", -1L)]
    [Arguments("2+3", 5L)]
    [Arguments("10-4", 6L)]
    [Arguments("3*4", 12L)]
    [Arguments("10/3", 3L)] // Integer division
    [Arguments("10%3", 1L)]
    [Arguments("(2+3)*4", 20L)]
    public async Task Evaluate_Long_Expression_Should_Return_Correct_Result(string expression, long expected)
    {
        var sut = DiceExpressionEvaluator<long>.CreateMinimum();
        
        var result = sut.Evaluate(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(expected);
    }
    
    [Test]
    public async Task Evaluate_Long_DiceRoll_Should_Return_Minimum()
    {
        var sut = DiceExpressionEvaluator<long>.CreateMinimum();
        
        var result = sut.Evaluate("2d6");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(2L);
    }
    
    [Test]
    public async Task Evaluate_Long_With_Variables_Should_Work()
    {
        var sut = DiceExpressionEvaluator<long>.CreateMinimum();
        var variables = new Dictionary<string, long> { ["STR"] = 5L };
        
        var result = sut.Evaluate("1d20+[STR]", variables);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(6L);
    }
    
    // Float type tests
    [Test]
    [Arguments("1", 1.0f)]
    [Arguments("2.5", 2.5f)]
    [Arguments("10.25", 10.25f)]
    [Arguments("-1.5", -1.5f)]
    [Arguments("2.5+3.5", 6.0f)]
    [Arguments("10.0-4.5", 5.5f)]
    [Arguments("3.0*4.0", 12.0f)]
    [Arguments("10.0/4.0", 2.5f)]
    [Arguments("(2.0+3.0)*4.0", 20.0f)]
    public async Task Evaluate_Float_Expression_Should_Return_Correct_Result(string expression, float expected)
    {
        var sut = DiceExpressionEvaluator<float>.CreateMinimum();
        
        var result = sut.Evaluate(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(expected);
    }
    
    [Test]
    public async Task Evaluate_Float_DiceRoll_Should_Return_Minimum()
    {
        var sut = DiceExpressionEvaluator<float>.CreateMinimum();
        
        var result = sut.Evaluate("2d6");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(2.0f);
    }
    
    [Test]
    public async Task Evaluate_Float_With_Variables_Should_Work()
    {
        var sut = DiceExpressionEvaluator<float>.CreateMinimum();
        var variables = new Dictionary<string, float> { ["MOD"] = 2.5f };
        
        var result = sut.Evaluate("1d20+[MOD]", variables);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(3.5f);
    }
    
    // Decimal type tests
    [Test]
    [Arguments("1", "1")]
    [Arguments("2.5", "2.5")]
    [Arguments("10.25", "10.25")]
    [Arguments("-1.5", "-1.5")]
    [Arguments("2.5+3.5", "6.0")]
    [Arguments("10.0-4.5", "5.5")]
    [Arguments("3.0*4.0", "12.00")]
    [Arguments("10.0/4.0", "2.5")]
    [Arguments("(2.0+3.0)*4.0", "20.00")]
    public async Task Evaluate_Decimal_Expression_Should_Return_Correct_Result(string expression, string expectedStr)
    {
        var expected = decimal.Parse(expectedStr);
        var sut = DiceExpressionEvaluator<decimal>.CreateMinimum();
        
        var result = sut.Evaluate(expression);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(expected);
    }
    
    [Test]
    public async Task Evaluate_Decimal_DiceRoll_Should_Return_Minimum()
    {
        var sut = DiceExpressionEvaluator<decimal>.CreateMinimum();
        
        var result = sut.Evaluate("2d6");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(2m);
    }
    
    [Test]
    public async Task Evaluate_Decimal_With_Variables_Should_Work()
    {
        var sut = DiceExpressionEvaluator<decimal>.CreateMinimum();
        var variables = new Dictionary<string, decimal> { ["BONUS"] = 1.5m };
        
        var result = sut.Evaluate("1d20+[BONUS]", variables);
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(2.5m);
    }
    
    // Math functions with different types
    [Test]
    public async Task Evaluate_Long_Math_Functions_Should_Work()
    {
        var sut = DiceExpressionEvaluator<long>.CreateMinimum();
        
        var floorResult = sut.Evaluate("floor(5)");
        var ceilResult = sut.Evaluate("ceil(5)");
        var absResult = sut.Evaluate("abs(-5)");
        var minResult = sut.Evaluate("min(3, 5)");
        var maxResult = sut.Evaluate("max(3, 5)");
        
        await Assert.That(floorResult.Value.Result).IsEqualTo(5L);
        await Assert.That(ceilResult.Value.Result).IsEqualTo(5L);
        await Assert.That(absResult.Value.Result).IsEqualTo(5L);
        await Assert.That(minResult.Value.Result).IsEqualTo(3L);
        await Assert.That(maxResult.Value.Result).IsEqualTo(5L);
    }
    
    [Test]
    public async Task Evaluate_Float_Math_Functions_Should_Work()
    {
        var sut = DiceExpressionEvaluator<float>.CreateMinimum();
        
        var floorResult = sut.Evaluate("floor(3.7)");
        var ceilResult = sut.Evaluate("ceil(3.2)");
        var roundResult = sut.Evaluate("round(3.5)");
        var absResult = sut.Evaluate("abs(-5.5)");
        var sqrtResult = sut.Evaluate("sqrt(4.0)");
        var minResult = sut.Evaluate("min(3.5, 5.5)");
        var maxResult = sut.Evaluate("max(3.5, 5.5)");
        
        await Assert.That(floorResult.Value.Result).IsEqualTo(3.0f);
        await Assert.That(ceilResult.Value.Result).IsEqualTo(4.0f);
        await Assert.That(roundResult.Value.Result).IsEqualTo(4.0f);
        await Assert.That(absResult.Value.Result).IsEqualTo(5.5f);
        await Assert.That(sqrtResult.Value.Result).IsEqualTo(2.0f);
        await Assert.That(minResult.Value.Result).IsEqualTo(3.5f);
        await Assert.That(maxResult.Value.Result).IsEqualTo(5.5f);
    }
    
    [Test]
    public async Task Evaluate_Decimal_Math_Functions_Should_Work()
    {
        var sut = DiceExpressionEvaluator<decimal>.CreateMinimum();
        
        var floorResult = sut.Evaluate("floor(3.7)");
        var ceilResult = sut.Evaluate("ceil(3.2)");
        var roundResult = sut.Evaluate("round(3.5)");
        var absResult = sut.Evaluate("abs(-5.5)");
        var minResult = sut.Evaluate("min(3.5, 5.5)");
        var maxResult = sut.Evaluate("max(3.5, 5.5)");
        
        await Assert.That(floorResult.Value.Result).IsEqualTo(3m);
        await Assert.That(ceilResult.Value.Result).IsEqualTo(4m);
        await Assert.That(roundResult.Value.Result).IsEqualTo(4m);
        await Assert.That(absResult.Value.Result).IsEqualTo(5.5m);
        await Assert.That(minResult.Value.Result).IsEqualTo(3.5m);
        await Assert.That(maxResult.Value.Result).IsEqualTo(5.5m);
    }

    [Test]
    public async Task Evaluate_Decimal_Round_Should_Use_Bankers_Rounding()
    {
        var sut = DiceExpressionEvaluator<decimal>.CreateMinimum();

        var result = sut.Evaluate("round(2.5)");

        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(2m);
    }
    
    // Conditional tests for new types
    [Test]
    public async Task Evaluate_Short_Conditional_Should_Work()
    {
        var sut = DiceExpressionEvaluator<short>.CreateMinimum();
        
        var result = sut.Evaluate("if(5 > 3, 10, 20)");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo((short)10);
    }
    
    [Test]
    public async Task Evaluate_Long_Conditional_Should_Work()
    {
        var sut = DiceExpressionEvaluator<long>.CreateMinimum();
        
        var result = sut.Evaluate("if(5 > 3, 10, 20)");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(10L);
    }
    
    [Test]
    public async Task Evaluate_Float_Conditional_Should_Work()
    {
        var sut = DiceExpressionEvaluator<float>.CreateMinimum();
        
        var result = sut.Evaluate("if(5.5 > 3.5, 10.5, 20.5)");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(10.5f);
    }
    
    [Test]
    public async Task Evaluate_Decimal_Conditional_Should_Work()
    {
        var sut = DiceExpressionEvaluator<decimal>.CreateMinimum();
        
        var result = sut.Evaluate("if(5.5 > 3.5, 10.5, 20.5)");
        
        await Assert.That(result.IsSuccess).IsTrue();
        await Assert.That(result.Value.Result).IsEqualTo(10.5m);
    }
    
    // All roller types should work with new numeric types
    [Test]
    public async Task All_Roller_Types_Should_Work_With_Short()
    {
        var randomEval = DiceExpressionEvaluator<short>.CreateRandom();
        var seededEval = DiceExpressionEvaluator<short>.CreateSeededRandom(42);
        var minEval = DiceExpressionEvaluator<short>.CreateMinimum();
        var maxEval = DiceExpressionEvaluator<short>.CreateMaximum();
        var avgEval = DiceExpressionEvaluator<short>.CreateFixedAverage();
        
        await Assert.That(randomEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(seededEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(minEval.Evaluate("1d6").Value.Result).IsEqualTo((short)1);
        await Assert.That(maxEval.Evaluate("1d6").Value.Result).IsEqualTo((short)6);
        await Assert.That(avgEval.Evaluate("1d6").Value.Result).IsEqualTo((short)4);
    }
    
    [Test]
    public async Task All_Roller_Types_Should_Work_With_Long()
    {
        var randomEval = DiceExpressionEvaluator<long>.CreateRandom();
        var seededEval = DiceExpressionEvaluator<long>.CreateSeededRandom(42);
        var minEval = DiceExpressionEvaluator<long>.CreateMinimum();
        var maxEval = DiceExpressionEvaluator<long>.CreateMaximum();
        var avgEval = DiceExpressionEvaluator<long>.CreateFixedAverage();
        
        await Assert.That(randomEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(seededEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(minEval.Evaluate("1d6").Value.Result).IsEqualTo(1L);
        await Assert.That(maxEval.Evaluate("1d6").Value.Result).IsEqualTo(6L);
        await Assert.That(avgEval.Evaluate("1d6").Value.Result).IsEqualTo(4L);
    }
    
    [Test]
    public async Task All_Roller_Types_Should_Work_With_Float()
    {
        var randomEval = DiceExpressionEvaluator<float>.CreateRandom();
        var seededEval = DiceExpressionEvaluator<float>.CreateSeededRandom(42);
        var minEval = DiceExpressionEvaluator<float>.CreateMinimum();
        var maxEval = DiceExpressionEvaluator<float>.CreateMaximum();
        var avgEval = DiceExpressionEvaluator<float>.CreateFixedAverage();
        
        await Assert.That(randomEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(seededEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(minEval.Evaluate("1d6").Value.Result).IsEqualTo(1.0f);
        await Assert.That(maxEval.Evaluate("1d6").Value.Result).IsEqualTo(6.0f);
        await Assert.That(avgEval.Evaluate("1d6").Value.Result).IsEqualTo(4.0f);
    }
    
    [Test]
    public async Task All_Roller_Types_Should_Work_With_Decimal()
    {
        var randomEval = DiceExpressionEvaluator<decimal>.CreateRandom();
        var seededEval = DiceExpressionEvaluator<decimal>.CreateSeededRandom(42);
        var minEval = DiceExpressionEvaluator<decimal>.CreateMinimum();
        var maxEval = DiceExpressionEvaluator<decimal>.CreateMaximum();
        var avgEval = DiceExpressionEvaluator<decimal>.CreateFixedAverage();
        
        await Assert.That(randomEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(seededEval.Evaluate("1d6").IsSuccess).IsTrue();
        await Assert.That(minEval.Evaluate("1d6").Value.Result).IsEqualTo(1m);
        await Assert.That(maxEval.Evaluate("1d6").Value.Result).IsEqualTo(6m);
        await Assert.That(avgEval.Evaluate("1d6").Value.Result).IsEqualTo(4m);
    }
    
    // TryParse tests for new types
    [Test]
    public async Task TryParse_Long_Should_Work()
    {
        var error = DiceExpressionParser.TryParse<long>("2d6+5", out var expression);
        
        await Assert.That(error).IsNull();
        await Assert.That(expression).IsNotNull();
    }
    
    [Test]
    public async Task TryParse_Float_Should_Work()
    {
        var error = DiceExpressionParser.TryParse<float>("2d6+5.5", out var expression);
        
        await Assert.That(error).IsNull();
        await Assert.That(expression).IsNotNull();
    }
    
    [Test]
    public async Task TryParse_Decimal_Should_Work()
    {
        var error = DiceExpressionParser.TryParse<decimal>("2d6+5.5", out var expression);
        
        await Assert.That(error).IsNull();
        await Assert.That(expression).IsNotNull();
    }
    
    [Test]
    public async Task TryParse_Short_Should_Work()
    {
        var error = DiceExpressionParser.TryParse<short>("2d6+5", out var expression);
        
        await Assert.That(error).IsNull();
        await Assert.That(expression).IsNotNull();
    }
    
    // Verify unsupported types return appropriate error
    [Test]
    public async Task Parse_UnsupportedType_Should_Return_Error()
    {
        var result = DiceExpressionParser.Parse<byte>("1+2");
        
        await Assert.That(result.IsFailure).IsTrue();
        await Assert.That(result.Error).IsTypeOf<ParserError>();
        var error = (ParserError)result.Error;
        await Assert.That(error.ErrorCode).IsEqualTo("Parsing.InvalidNumberType");
        await Assert.That(error.Message).Contains("Byte");
    }
}