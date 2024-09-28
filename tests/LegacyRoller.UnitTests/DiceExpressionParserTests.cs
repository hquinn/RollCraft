using LegacyRoller.UnitTests.Helpers;

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
    public async Task Should_Parse_Input_Into_Dice_Expression(string input, string expected)
    {
        var result = DiceExpressionParser.Parse(input);
        var actual = result.GetSuccessValue().ToString();
        
        await Assert.That(actual).IsEqualTo(expected);
    }
}