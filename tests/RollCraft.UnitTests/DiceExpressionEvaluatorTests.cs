using LitePrimitives;
using RollCraft.UnitTests.Helpers;

namespace RollCraft.UnitTests;

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
    [Arguments("1d6min3", 3.0)]
    [Arguments("1d6MIN3", 3.0)]
    [Arguments("1d6min3+3", 6.0)]
    [Arguments("4d6max3", 9.0)]
    [Arguments("4d6MAX3", 9.0)]
    [Arguments("4d6max3+3", 12.0)]
    [Arguments("4d6max(1+2)+3", 12.0)]
    [Arguments("6d6min2max4", 19.0)]
    [Arguments("10d10!", 56.0)]
    [Arguments("1d1!", 1002.0)]
    [Arguments("10d10!=5", 56.0)]
    [Arguments("10d10!=5", 56.0)]
    [Arguments("10d10!<>5", 510.0)]
    [Arguments("10d10!>5", 70.0)]
    [Arguments("10d10!<5", 91.0)]
    [Arguments("10d10!>=5", 113.0)]
    [Arguments("10d10!<=5", 110.0)]
    [Arguments("10d10!=(2*2)", 56.0)]
    [Arguments("4d6k3", 9.0)]
    [Arguments("4d6kh3", 9.0)]
    [Arguments("4d6kl3", 6.0)]
    [Arguments("10d10r", 56.0)]
    [Arguments("10d10r=5", 51.0)]
    [Arguments("10d10r<>5", 50.0)]
    [Arguments("10d10r>5", 30.0)]
    [Arguments("10d10r<5", 71.0)]
    [Arguments("10d10r>=5", 23.0)]
    [Arguments("10d10r<=5", 80.0)]
    [Arguments("10d10r=(2*2)", 52.0)]
    [Arguments("10d10ro", 55.0)]
    [Arguments("10d10ro=5", 51.0)]
    [Arguments("10d10ro<>5", 50.0)]
    [Arguments("10d10ro>5", 30.0)]
    [Arguments("10d10ro<5", 55.0)]
    [Arguments("10d10ro>=5", 31.0)]
    [Arguments("10d10ro<=5", 55.0)]
    [Arguments("10d10ro=(2*2)", 52.0)]
    [Arguments("1+2--3*4/5*-6-7+8*9/10--11/12*13+14*-15", -209.28333333333333)]
    [Arguments("4d10min2max8!=4r=5kh2+5", 15.0)]
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
    [Arguments("1d6!=1.1", "ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!=0", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!=-1", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!=7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!>1.1", "ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!>0", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>-1", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!>=1.1", "ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!>=0", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>=-1", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>=7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<1.1", "ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!<0", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<-1", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<=1.1", "ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!<=0", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<=-1", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<=7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<>1.1", "ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!<>0", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<>-1", "ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<>7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<>7", "ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6k1.1", "KeepError", "Keep must be an integer!")]
    [Arguments("1d6k-1", "KeepError", "Keep must be zero or more!")]
    [Arguments("1d6k7", "KeepError", "Keep must be less or equal than number of dice rolled!")]
    [Arguments("1d6kh1.1", "KeepError", "Keep must be an integer!")]
    [Arguments("1d6kh-1", "KeepError", "Keep must be zero or more!")]
    [Arguments("1d6kh7", "KeepError", "Keep must be less or equal than number of dice rolled!")]
    [Arguments("1d6kl1.1", "KeepError", "Keep must be an integer!")]
    [Arguments("1d6kl-1", "KeepError", "Keep must be zero or more!")]
    [Arguments("1d6kl7", "KeepError", "Keep must be less or equal than number of dice rolled!")]
    [Arguments("1d6max1.1", "MaximumError", "Maximum must be an integer!")]
    [Arguments("1d6max0", "MaximumError", "Cannot have a maximum value less than 1!")]
    [Arguments("1d6max-1", "MaximumError", "Cannot have a maximum value less than 1!")]
    [Arguments("1d6max7", "MaximumError", "Cannot have a maximum value greater than the dice side count!")]
    [Arguments("1d6min1.1", "MinimumError", "Minimum must be an integer!")]
    [Arguments("1d6min0", "MinimumError", "Cannot have a minimum value less than 1!")]
    [Arguments("1d6min-1", "MinimumError", "Cannot have a minimum value less than 1!")]
    [Arguments("1d6min7", "MinimumError", "Cannot have a minimum value greater than the dice side count!")]
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

    [Test]
    public async Task Should_Return_Minimum_Rolls_When_Using_Minimum_Roller()
    {
        const int expected = 4;
        var sut = DiceExpressionEvaluator<double>.CreateMinimum();
        var result = sut.Evaluate("4d6");
        
        await result.PerformAsync(
            success: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            failure: error => Assert.Fail(error.First().Message));
    }

    [Test]
    public async Task Should_Return_Maximum_Rolls_When_Using_Maximum_Roller()
    {
        const int expected = 24;
        var sut = DiceExpressionEvaluator<double>.CreateMaximum();
        var result = sut.Evaluate("4d6");
        
        await result.PerformAsync(
            success: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            failure: error => Assert.Fail(error.First().Message));
    }

    [Test]
    public async Task Should_Return_Fixed_Average_Rolls_When_Using_Fixed_Average_Roller()
    {
        const int expected = 16;
        var sut = DiceExpressionEvaluator<double>.CreateFixedAverage();
        var result = sut.Evaluate("4d6");
        
        await result.PerformAsync(
            success: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            failure: error => Assert.Fail(error.First().Message));
    }

    [Test]
    public async Task Should_Return_Random_Rolls_When_Using_Random_Roller()
    {
        var sut = DiceExpressionEvaluator<double>.CreateRandom();
        var result = sut.Evaluate("100d2");
        
        await result.PerformAsync(
            success: async actual =>
            {
                await using var _ = Assert.Multiple();

                await Assert.That(actual.Rolls.MaxBy(x => x.Roll)!.Roll).IsEqualTo(2);
                await Assert.That(actual.Rolls.MinBy(x => x.Roll)!.Roll).IsEqualTo(1);
            },
            failure: error => Assert.Fail(error.First().Message));
    }

    [Test]
    [MethodDataSource(nameof(RollsTestMethodData))]
    public async Task Should_Return_Rolls_In_Result_Simple(RollsTestData data)
    {
        var result = Evaluate(data.Input, data.UseMaxRoller);
        
        await result.PerformAsync(
            success: async actual =>
            {
                await using var _ = Assert.Multiple();

                await Assert.That(actual.Rolls).IsEquivalentCollectionTo(data.Expected);
            },
            failure: error => Assert.Fail(error.First().Message));
    }

    private static Result<DiceExpressionResult<double>> Evaluate(string input, bool useMaxRoller = false)
    {
        var sut = useMaxRoller 
            ? DiceExpressionEvaluator<double>.CreateMaximum() 
            : DiceExpressionEvaluator<double>.CreateCustom(new SequentialRoller());
        var result = sut.Evaluate(input);
        return result;
    }
    
    public record RollsTestData(string Input, List<DiceRoll> Expected, bool UseMaxRoller = false);

    public static IEnumerable<RollsTestData> RollsTestMethodData()
    {
        yield return new RollsTestData("4d6", 
        [
            new(6, 1), 
            new(6, 2), 
            new(6, 3), 
            new(6, 4)
        ]);
        
        yield return new RollsTestData("(1d4)d6", 
        [
            new(4, 1), 
            new(6, 2)
        ]);
        
        yield return new RollsTestData("(1d4)d(1d6)", 
        [
            new(4, 1), 
            new(6, 2),
            new(2, 1)
        ]);
        
        yield return new RollsTestData("4d6k(1d4)", 
        [
            new(6, 1, DiceModifier.Dropped), 
            new(6, 2, DiceModifier.Dropped), 
            new(6, 3, DiceModifier.Dropped), 
            new(6, 4), 
            new(4, 1)
        ]);   
        
        yield return new RollsTestData("1d6+1d8-1d10*1d12/1d20", 
        [
            new(6, 1), 
            new(8, 2), 
            new(10, 3), 
            new(12, 4), 
            new(20, 5)
        ]);
        
        yield return new RollsTestData("(1d6)d(1d10)k(1d4)!=(1d8)", 
        [
            new(6, 6), 
            new(10, 10), 
            new(10, 10), 
            new(10, 10), 
            new(10, 10), 
            new(10, 10), 
            new(10, 10, DiceModifier.Dropped), 
            new(10, 10, DiceModifier.Dropped), 
            new(4, 4),
            new(8, 8)
        ], UseMaxRoller: true);
    }
}