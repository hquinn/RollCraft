namespace LegacyRoller.UnitTests;

public class DiceExpressionParserTests
{
    [Test]
    [Arguments("1", "NUMBER(1)")]
    [Arguments("2", "NUMBER(2)")]
    public async Task Should_Parse_Input_Into_Dice_Expression(string input, string expected)
    {
        var result = DiceExpressionParser.Parse(input);
        await Assert.That(result.ToString()).IsEqualTo(expected);
    }
}