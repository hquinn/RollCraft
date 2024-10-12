namespace LegacyRoller.UnitTests;

public class DiceExpressionEvaluatorTests
{
    [Test]
    [Arguments("1", 1.0)]
    [Arguments("2", 2.0)]
    [Arguments("10", 10.0)]
    [Arguments("6.9", 6.9)]
    public async Task Should_Return_Correct_Result_From_DiceExpression(string input, double expected)
    {
        var sut = new DiceExpressionEvaluator();
        var result = sut.Evaluate(input);

        await result.PerformAsync(
            success: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            failure: error => Assert.Fail(error.First().Message));
    }
}