namespace LegacyRoller.UnitTests;

public class DiceExpressionParserTests
{
    [Test]
    [Arguments("1", "NUMBER(1)")]
    [Arguments("2", "NUMBER(2)")]
    [Arguments("10", "NUMBER(10)")]
    [Arguments("6.9", "NUMBER(6.9)")]
    [Arguments("-1", "UNARY(NUMBER(1))")]
    [Arguments("-2", "UNARY(NUMBER(2))")]
    [Arguments("-10", "UNARY(NUMBER(10))")]
    [Arguments("-6.9", "UNARY(NUMBER(6.9))")]
    [Arguments("--1", "UNARY(UNARY(NUMBER(1)))")]
    [Arguments("---1", "UNARY(UNARY(UNARY(NUMBER(1))))")]
    [Arguments("2-1", "SUBTRACT(NUMBER(2), NUMBER(1))")]
    [Arguments("-2-1", "SUBTRACT(UNARY(NUMBER(2)), NUMBER(1))")]
    [Arguments("2+1", "ADD(NUMBER(2), NUMBER(1))")]
    [Arguments("-2+1", "ADD(UNARY(NUMBER(2)), NUMBER(1))")]
    [Arguments("2+1-1", "SUBTRACT(ADD(NUMBER(2), NUMBER(1)), NUMBER(1))")]
    [Arguments("2*1", "MULTIPLY(NUMBER(2), NUMBER(1))")]
    [Arguments("2*1+1", "ADD(MULTIPLY(NUMBER(2), NUMBER(1)), NUMBER(1))")]
    [Arguments("2+1*1", "ADD(NUMBER(2), MULTIPLY(NUMBER(1), NUMBER(1)))")]
    [Arguments("2*-1+1", "ADD(MULTIPLY(NUMBER(2), UNARY(NUMBER(1))), NUMBER(1))")]
    [Arguments("2+1*-1", "ADD(NUMBER(2), MULTIPLY(NUMBER(1), UNARY(NUMBER(1))))")]
    [Arguments("2/1", "DIVIDE(NUMBER(2), NUMBER(1))")]
    [Arguments("2/1+1", "ADD(DIVIDE(NUMBER(2), NUMBER(1)), NUMBER(1))")]
    [Arguments("2+1/1", "ADD(NUMBER(2), DIVIDE(NUMBER(1), NUMBER(1)))")]
    [Arguments("2/-1+1", "ADD(DIVIDE(NUMBER(2), UNARY(NUMBER(1))), NUMBER(1))")]
    [Arguments("2+1/-1", "ADD(NUMBER(2), DIVIDE(NUMBER(1), UNARY(NUMBER(1))))")]
    [Arguments("2+1/-1*2", "ADD(NUMBER(2), MULTIPLY(DIVIDE(NUMBER(1), UNARY(NUMBER(1))), NUMBER(2)))")]
    [Arguments("d6", "DICE(NUMBER(1), NUMBER(6))")]
    [Arguments("D6", "DICE(NUMBER(1), NUMBER(6))")]
    [Arguments("1d6", "DICE(NUMBER(1), NUMBER(6))")]
    [Arguments("2d6", "DICE(NUMBER(2), NUMBER(6))")]
    [Arguments("-2d6", "DICE(UNARY(NUMBER(2)), NUMBER(6))")]
    [Arguments("-1d6", "DICE(UNARY(NUMBER(1)), NUMBER(6))")]
    [Arguments("-d6", "DICE(UNARY(NUMBER(1)), NUMBER(6))")]
    [Arguments("1d6+3", "ADD(DICE(NUMBER(1), NUMBER(6)), NUMBER(3))")]
    public async Task Should_Parse_Input_Into_Dice_Expression(string input, string expected)
    {
        var result = DiceExpressionParser.Parse(input);

        await result.PerformAsync(
            success: async actual => await Assert.That(actual.ToString()).IsEqualTo(expected),
            failure: async error => await Task.Run(() => Assert.Fail(error.First().Message)));
    }
}