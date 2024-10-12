namespace LegacyRoller.UnitTests;

public class DiceExpressionParserTests
{
    [Test]
    [Arguments("1", "1")]
    [Arguments("2", "2")]
    [Arguments("10", "10")]
    [Arguments("6.9", "6.9")]
    [Arguments("-1", "UNARY(1)")]
    [Arguments("-2", "UNARY(2)")]
    [Arguments("-10", "UNARY(10)")]
    [Arguments("-6.9", "UNARY(6.9)")]
    [Arguments("--1", "UNARY(UNARY(1))")]
    [Arguments("---1", "UNARY(UNARY(UNARY(1)))")]
    [Arguments("2-1", "SUBTRACT(2, 1)")]
    [Arguments("-2-1", "SUBTRACT(UNARY(2), 1)")]
    [Arguments("2+1", "ADD(2, 1)")]
    [Arguments("-2+1", "ADD(UNARY(2), 1)")]
    [Arguments("2+1-1", "SUBTRACT(ADD(2, 1), 1)")]
    [Arguments("2*1", "MULTIPLY(2, 1)")]
    [Arguments("2*1+1", "ADD(MULTIPLY(2, 1), 1)")]
    [Arguments("2+1*1", "ADD(2, MULTIPLY(1, 1))")]
    [Arguments("2*-1+1", "ADD(MULTIPLY(2, UNARY(1)), 1)")]
    [Arguments("2+1*-1", "ADD(2, MULTIPLY(1, UNARY(1)))")]
    [Arguments("2/1", "DIVIDE(2, 1)")]
    [Arguments("2/1+1", "ADD(DIVIDE(2, 1), 1)")]
    [Arguments("2+1/1", "ADD(2, DIVIDE(1, 1))")]
    [Arguments("2/-1+1", "ADD(DIVIDE(2, UNARY(1)), 1)")]
    [Arguments("2+1/-1", "ADD(2, DIVIDE(1, UNARY(1)))")]
    [Arguments("2+1/-1*2", "ADD(2, MULTIPLY(DIVIDE(1, UNARY(1)), 2))")]
    [Arguments("d6", "DICE(1, 6)")]
    [Arguments("D6", "DICE(1, 6)")]
    [Arguments("1d6", "DICE(1, 6)")]
    [Arguments("2d6", "DICE(2, 6)")]
    [Arguments("-2d6", "DICE(UNARY(2), 6)")]
    [Arguments("-1d6", "DICE(UNARY(1), 6)")]
    [Arguments("-d6", "DICE(UNARY(1), 6)")]
    [Arguments("1d6+3", "ADD(DICE(1, 6), 3)")]
    [Arguments("(1+2)*3", "MULTIPLY(ADD(1, 2), 3)")]
    [Arguments("(1+(2+3))*3", "MULTIPLY(ADD(1, ADD(2, 3)), 3)")]
    [Arguments("(1+2+3)*3", "MULTIPLY(ADD(ADD(1, 2), 3), 3)")]
    [Arguments("1d(2*3)", "DICE(1, MULTIPLY(2, 3))")]
    [Arguments("1d6min3", "DICE(1, 6, MINIMUM=3)")]
    [Arguments("1d6MIN3", "DICE(1, 6, MINIMUM=3)")]
    [Arguments("1d6min-3", "DICE(1, 6, MINIMUM=UNARY(3))")]
    [Arguments("1d-6min-3", "DICE(1, UNARY(6), MINIMUM=UNARY(3))")]
    [Arguments("1d6min3+3", "ADD(DICE(1, 6, MINIMUM=3), 3)")]
    [Arguments("1d6max3", "DICE(1, 6, MAXIMUM=3)")]
    [Arguments("1d6MAX3", "DICE(1, 6, MAXIMUM=3)")]
    [Arguments("1d6max-3", "DICE(1, 6, MAXIMUM=UNARY(3))")]
    [Arguments("1d-6max-3", "DICE(1, UNARY(6), MAXIMUM=UNARY(3))")]
    [Arguments("1d6max3+3", "ADD(DICE(1, 6, MAXIMUM=3), 3)")]
    [Arguments("1d6max(3+3)+3", "ADD(DICE(1, 6, MAXIMUM=ADD(3, 3)), 3)")]
    [Arguments("1d6min2max4", "DICE(1, 6, MINIMUM=2, MAXIMUM=4)")]
    [Arguments("1d6min-2max4", "DICE(1, 6, MINIMUM=UNARY(2), MAXIMUM=4)")]
    [Arguments("1d6!", "DICE(1, 6, EXPLODING=MAX)")]
    [Arguments("1d6!=5", "DICE(1, 6, EXPLODING=EQUAL(5))")]
    [Arguments("1d6!<>5", "DICE(1, 6, EXPLODING=NOTEQUAL(5))")]
    [Arguments("1d6!>5", "DICE(1, 6, EXPLODING=GREATERTHAN(5))")]
    [Arguments("1d6!<5", "DICE(1, 6, EXPLODING=LESSTHAN(5))")]
    [Arguments("1d6!>=5", "DICE(1, 6, EXPLODING=GREATERTHANEQUAL(5))")]
    [Arguments("1d6!<=5", "DICE(1, 6, EXPLODING=LESSTHANEQUAL(5))")]
    [Arguments("1d6!=(5*5)", "DICE(1, 6, EXPLODING=EQUAL(MULTIPLY(5, 5)))")]
    [Arguments("4d6k3", "DICE(4, 6, KEEPHIGHEST=3)")]
    [Arguments("4d6kh3", "DICE(4, 6, KEEPHIGHEST=3)")]
    [Arguments("4d6kl3", "DICE(4, 6, KEEPLOWEST=3)")]
    [Arguments("1d6r", "DICE(1, 6, REROLL=MAX)")]
    [Arguments("1d6r=5", "DICE(1, 6, REROLL=EQUAL(5))")]
    [Arguments("1d6r<>5", "DICE(1, 6, REROLL=NOTEQUAL(5))")]
    [Arguments("1d6r>5", "DICE(1, 6, REROLL=GREATERTHAN(5))")]
    [Arguments("1d6r<5", "DICE(1, 6, REROLL=LESSTHAN(5))")]
    [Arguments("1d6r>=5", "DICE(1, 6, REROLL=GREATERTHANEQUAL(5))")]
    [Arguments("1d6r<=5", "DICE(1, 6, REROLL=LESSTHANEQUAL(5))")]
    [Arguments("1d6r=(5*5)", "DICE(1, 6, REROLL=EQUAL(MULTIPLY(5, 5)))")]
    [Arguments("1d6ro", "DICE(1, 6, REROLLONCE=MAX)")]
    [Arguments("1d6ro=5", "DICE(1, 6, REROLLONCE=EQUAL(5))")]
    [Arguments("1d6ro<>5", "DICE(1, 6, REROLLONCE=NOTEQUAL(5))")]
    [Arguments("1d6ro>5", "DICE(1, 6, REROLLONCE=GREATERTHAN(5))")]
    [Arguments("1d6ro<5", "DICE(1, 6, REROLLONCE=LESSTHAN(5))")]
    [Arguments("1d6ro>=5", "DICE(1, 6, REROLLONCE=GREATERTHANEQUAL(5))")]
    [Arguments("1d6ro<=5", "DICE(1, 6, REROLLONCE=LESSTHANEQUAL(5))")]
    [Arguments("1d6ro=(5*5)", "DICE(1, 6, REROLLONCE=EQUAL(MULTIPLY(5, 5)))")]
    public async Task Should_Parse_Input_Into_Dice_Expression(string input, string expected)
    {
        var result = DiceExpressionParser.Parse(input);

        await result.PerformAsync(
            success: async actual => await Assert.That(actual.ToString()).IsEqualTo(expected),
            failure: async error => await Task.Run(() => Assert.Fail(error.First().Message)));
    }
    
    [Test]
    [Arguments("x", "InvalidToken", "Invalid token found", 0)]
    [Arguments("^", "InvalidToken", "Invalid token found", 0)]
    [Arguments("1.", "InvalidToken", "Invalid token found", 0)]
    [Arguments("1..", "InvalidToken", "Invalid token found", 0)]
    [Arguments("1.0.", "InvalidToken", "Invalid token found", 3)]
    [Arguments("", "UnexpectedEnd", "Unexpected end of input", 0)]
    [Arguments("-", "UnexpectedEnd", "Unexpected end of input", 1)]
    [Arguments("--", "UnexpectedEnd", "Unexpected end of input", 2)]
    [Arguments("(", "UnexpectedEnd", "Unexpected end of input", 1)]
    [Arguments("1=", "UnexpectedEnd", "Unexpected end of input", 2)]
    [Arguments("()", "UnexpectedRightParen", "Unexpected closing parenthesis", 2)]
    [Arguments("1 1", "UnexpectedToken", "Unexpected token 'Number' at position 1", 1)]
    [Arguments("*", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("+", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("/", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("min3", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("max3", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("r", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("ro", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("!", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("k", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("kh", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("kl", "InvalidPrefix", "Invalid prefix found", 1)]
    [Arguments("1+*", "InvalidPrefix", "Invalid prefix found", 3)]
    [Arguments("1=1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1<>1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1>1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1>=1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1<1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1<=1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1min1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1max1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1r1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1ro1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1!1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1k1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1kh1", "InvalidInfix", "Invalid infix found", 3)]
    [Arguments("1kl1", "InvalidInfix", "Invalid infix found", 3)]
    public async Task Should_Return_Parser_Error(string input, string expectedCode, string expectedMessage, int expectedIndex)
    {
        var result = DiceExpressionParser.Parse(input);

        await result.PerformAsync(
            success: async success => await Task.Run(() => Assert.Fail($"Expected a failure, but got {success}")),
            failure: async error =>
            {
                await using var _ = Assert.Multiple();
                
                await Assert.That(error.First().Code).IsEqualTo(expectedCode);
                await Assert.That(error.First().Message).IsEqualTo(expectedMessage);
                await Assert.That((int)error.First().Metadata["Position"]).IsEqualTo(expectedIndex);
            });
    }
}