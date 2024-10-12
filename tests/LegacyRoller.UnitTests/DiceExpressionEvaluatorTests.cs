using LegacyRoller.UnitTests.Helpers;
using LitePrimitives;

namespace LegacyRoller.UnitTests;

public class DiceExpressionEvaluatorTests
{
    [Test]
    [Arguments("1", 1.0)]
    [Arguments("2", 2.0)]
    [Arguments("10", 10.0)]
    [Arguments("6.9", 6.9)]
    [Arguments("-1", -1.0)]
    [Arguments("-2", -2.0)]
    [Arguments("-10", -10.0)]
    [Arguments("-6.9", -6.9)]
    [Arguments("--1", 1.0)]
    [Arguments("---1", -1.0)]
    [Arguments("2-1", 1.0)]
    [Arguments("-2-1", -3.0)]
    [Arguments("2+1", 3.0)]
    [Arguments("-2+1", -1.0)]
    [Arguments("2+1-1", 2.0)]
    [Arguments("2*1", 2.0)]
    [Arguments("2*3+4", 10.0)]
    [Arguments("2+3*4", 14.0)]
    [Arguments("2*-3+4", -2.0)]
    [Arguments("2+3*-4", -10.0)]
    [Arguments("4/2", 2.0)]
    [Arguments("4/2+3", 5.0)]
    [Arguments("2+4/3", 3.333333333333333)]
    [Arguments("2/-1+1", -1.0)]
    [Arguments("2+1/-1", 1.0)]
    [Arguments("2+1/-1*2", 0.0)]
    [Arguments("(1+2)*3", 9.0)]
    [Arguments("(1+(2+3))*3", 18.0)]
    [Arguments("(1+2+3)*3", 18.0)]
    [Arguments("d6", 1.0)]
    [Arguments("D6", 1.0)]
    [Arguments("1d6", 1.0)]
    [Arguments("2d6", 3.0)]
    [Arguments("-2d6", -3.0)]
    [Arguments("-1d6", -1.0)]
    [Arguments("-d6", -1.0)]
    [Arguments("1d6+3", 4.0)]
    [Arguments("1d(2*3)", 1.0)]
    public async Task Should_Return_Correct_Result_From_DiceExpression_For_Simple_Math(string input, double expected)
    {
        var result = Evaluate(input);

        await result.PerformAsync(
            success: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            failure: error => Assert.Fail(error.First().Message));
    }

    [Test]
    [Arguments("1/0", "DivideByZero", "Division by zero detected!")]
    [Arguments("1.1d6", "DiceError", "Dice count must be an integer!")]
    [Arguments("1d6.1", "DiceError", "Dice sides must be an integer!")]
    [Arguments("0d6", "DiceError", "Dice count must not be 0!")]
    [Arguments("1d0", "DiceError", "Dice sides must not be 0 or less!")]
    [Arguments("1d-1", "DiceError", "Dice sides must not be 0 or less!")]
    public async Task Should_Return_Evaluator_Error(string input, string expectedCode, string expectedMessage)
    {
        var result = Evaluate(input);

        await result.PerformAsync(
            success: success => Assert.Fail($"Expected a failure, but got {success}"),
            failure: async error =>
            {
                await using var _ = Assert.Multiple();
                
                await Assert.That(error.First().Code).IsEqualTo(expectedCode);
                await Assert.That(error.First().Message).IsEqualTo(expectedMessage);
            });
    }

    private static Result<DiceExpressionResult> Evaluate(string input)
    {
        var sut = new DiceExpressionEvaluator(new SequentialRandom());
        var result = sut.Evaluate(input);
        return result;
    }
}