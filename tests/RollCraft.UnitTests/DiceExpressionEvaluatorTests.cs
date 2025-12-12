using MonadCraft;
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

        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }

    [Test]
    [Arguments("1/0", "Evaluator.DivideByZero", "Division by zero detected!")]
    [Arguments("1.1d6", "Evaluator.DiceError", "Dice count must be an integer!")]
    [Arguments("1d6.1", "Evaluator.DiceError", "Dice sides must be an integer!")]
    [Arguments("0d6", "Evaluator.DiceError", "Dice count must not be 0!")]
    [Arguments("1d0", "Evaluator.DiceError", "Dice sides must not be 0 or less!")]
    [Arguments("1d-1", "Evaluator.DiceError", "Dice sides must not be 0 or less!")]
    [Arguments("1d6!=1.1", "Evaluator.ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!=0", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!=-1", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!=7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!>1.1", "Evaluator.ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!>0", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>-1", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!>=1.1", "Evaluator.ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!>=0", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>=-1", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!>=7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<1.1", "Evaluator.ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!<0", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<-1", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<=1.1", "Evaluator.ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!<=0", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<=-1", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<=7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<>1.1", "Evaluator.ComparisonError", "Comparison must be an integer!")]
    [Arguments("1d6!<>0", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<>-1", "Evaluator.ComparisonError", "Comparison must not be less than 1!")]
    [Arguments("1d6!<>7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6!<>7", "Evaluator.ComparisonError", "Comparison must not be greater than the dice side count!")]
    [Arguments("1d6k1.1", "Evaluator.KeepError", "Keep must be an integer!")]
    [Arguments("1d6k-1", "Evaluator.KeepError", "Keep must be zero or more!")]
    [Arguments("1d6k7", "Evaluator.KeepError", "Keep must be less or equal than number of dice rolled!")]
    [Arguments("1d6kh1.1", "Evaluator.KeepError", "Keep must be an integer!")]
    [Arguments("1d6kh-1", "Evaluator.KeepError", "Keep must be zero or more!")]
    [Arguments("1d6kh7", "Evaluator.KeepError", "Keep must be less or equal than number of dice rolled!")]
    [Arguments("1d6kl1.1", "Evaluator.KeepError", "Keep must be an integer!")]
    [Arguments("1d6kl-1", "Evaluator.KeepError", "Keep must be zero or more!")]
    [Arguments("1d6kl7", "Evaluator.KeepError", "Keep must be less or equal than number of dice rolled!")]
    [Arguments("1d6max1.1", "Evaluator.MaximumError", "Maximum must be an integer!")]
    [Arguments("1d6max0", "Evaluator.MaximumError", "Cannot have a maximum value less than 1!")]
    [Arguments("1d6max-1", "Evaluator.MaximumError", "Cannot have a maximum value less than 1!")]
    [Arguments("1d6max7", "Evaluator.MaximumError", "Cannot have a maximum value greater than the dice side count!")]
    [Arguments("1d6min1.1", "Evaluator.MinimumError", "Minimum must be an integer!")]
    [Arguments("1d6min0", "Evaluator.MinimumError", "Cannot have a minimum value less than 1!")]
    [Arguments("1d6min-1", "Evaluator.MinimumError", "Cannot have a minimum value less than 1!")]
    [Arguments("1d6min7", "Evaluator.MinimumError", "Cannot have a minimum value greater than the dice side count!")]
    public async Task Should_Return_Evaluator_Error(string input, string expectedCode, string expectedMessage)
    {
        var result = Evaluate(input);

        await result.SwitchAsync(
            onSuccess: success => Assert.Fail($"Expected a failure, but got {success}"),
            onFailure: async error =>
            {
                using var _ = Assert.Multiple();
                
                await Assert.That(error.ErrorCode).IsEqualTo(expectedCode);
                await Assert.That(error.Message).IsEqualTo(expectedMessage);
            });
    }

    [Test]
    public async Task Should_Return_Minimum_Rolls_When_Using_Minimum_Roller()
    {
        const int expected = 4;
        var sut = DiceExpressionEvaluator<double>.CreateMinimum();
        var result = sut.Evaluate("4d6");
        
        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }

    [Test]
    public async Task Should_Return_Maximum_Rolls_When_Using_Maximum_Roller()
    {
        const int expected = 24;
        var sut = DiceExpressionEvaluator<double>.CreateMaximum();
        var result = sut.Evaluate("4d6");
        
        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }

    [Test]
    public async Task Should_Return_Fixed_Average_Rolls_When_Using_Fixed_Average_Roller()
    {
        const int expected = 16;
        var sut = DiceExpressionEvaluator<double>.CreateFixedAverage();
        var result = sut.Evaluate("4d6");
        
        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }

    [Test]
    public async Task Should_Return_Random_Rolls_When_Using_Random_Roller()
    {
        var sut = DiceExpressionEvaluator<double>.CreateRandom();
        var result = sut.Evaluate("100d2");
        
        await result.SwitchAsync(
            onSuccess: async actual =>
            {
                using var _ = Assert.Multiple();

                await Assert.That(actual.Rolls.MaxBy(x => x.Roll)!.Roll).IsEqualTo(2);
                await Assert.That(actual.Rolls.MinBy(x => x.Roll)!.Roll).IsEqualTo(1);
            },
            onFailure: error => Assert.Fail(error.Message));
    }

    [Test]
    [MethodDataSource(nameof(RollsTestMethodData))]
    public async Task Should_Return_Rolls_In_Result_Simple(RollsTestData data)
    {
        var result = Evaluate(data.Input, RollerType.Exact, data.Rolls);
        
        await result.SwitchAsync(
            onSuccess: async actual =>
            {
                using var _ = Assert.Multiple();

                await Assert.That(actual.Rolls).IsEquivalentTo(data.Expected);
            },
            onFailure: error => Assert.Fail(error.Message));
    }

    [Test]
    [Arguments("1d6 + [STR]", 5, 6.0)]
    [Arguments("[STR]", 5, 5.0)]
    [Arguments("[STR] + [DEX]", 5, 8.0)]
    [Arguments("[STR] * 2", 5, 10.0)]
    [Arguments("2 * [STR]", 5, 10.0)]
    [Arguments("[STR] + [STR]", 5, 10.0)]
    [Arguments("-[STR]", 5, -5.0)]
    [Arguments("[STR] - 2", 5, 3.0)]
    [Arguments("[STR] / 2", 4, 2.0)]
    [Arguments("([STR] + 1) * 2", 5, 12.0)]
    [Arguments("[MODIFIER]d6", 2, 3.0)]
    [Arguments("1d[SIDES]", 8, 1.0)]
    public async Task Should_Return_Correct_Result_From_DiceExpression_With_Variables(string input, double strValue, double expected)
    {
        var variables = new Dictionary<string, double>
        {
            ["STR"] = strValue,
            ["DEX"] = 3.0,
            ["MODIFIER"] = 2.0,
            ["SIDES"] = 8.0
        };
        
        var result = EvaluateWithVariables(input, variables);

        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }
    
    [Test]
    public async Task Should_Return_Error_When_Variable_Not_Defined()
    {
        var variables = new Dictionary<string, double>();
        var result = EvaluateWithVariables("[UNDEFINED]", variables);
        
        await result.SwitchAsync(
            onSuccess: success => Assert.Fail($"Expected a failure, but got {success}"),
            onFailure: async error =>
            {
                using var _ = Assert.Multiple();
                
                await Assert.That(error.ErrorCode).IsEqualTo("Evaluator.UndefinedVariable");
                await Assert.That(error.Message).IsEqualTo("Variable 'UNDEFINED' is not defined!");
            });
    }
    
    [Test]
    public async Task Should_Return_Error_When_Variable_Not_Resolved()
    {
        // Evaluating without providing variables dictionary should fail for expressions with variables
        var result = Evaluate("[STR]");
        
        await result.SwitchAsync(
            onSuccess: success => Assert.Fail($"Expected a failure, but got {success}"),
            onFailure: async error =>
            {
                using var _ = Assert.Multiple();
                
                await Assert.That(error.ErrorCode).IsEqualTo("Evaluator.UnresolvedVariable");
                await Assert.That(error.Message).IsEqualTo("Variable 'STR' was not resolved before evaluation!");
            });
    }

    [Test]
    [Arguments("if(1 = 1, 10, 20)", 10.0)]
    [Arguments("if(1 = 2, 10, 20)", 20.0)]
    [Arguments("if(5 > 3, 100, 0)", 100.0)]
    [Arguments("if(3 > 5, 100, 0)", 0.0)]
    [Arguments("if(5 >= 5, 100, 0)", 100.0)]
    [Arguments("if(4 >= 5, 100, 0)", 0.0)]
    [Arguments("if(3 < 5, 100, 0)", 100.0)]
    [Arguments("if(5 < 3, 100, 0)", 0.0)]
    [Arguments("if(5 <= 5, 100, 0)", 100.0)]
    [Arguments("if(6 <= 5, 100, 0)", 0.0)]
    [Arguments("if(1 <> 2, 100, 0)", 100.0)]
    [Arguments("if(2 <> 2, 100, 0)", 0.0)]
    [Arguments("if(1+1 = 2, 10, 20)", 10.0)]
    [Arguments("if(2*3 >= 5, 100, 0)", 100.0)]
    [Arguments("if(1 = 1, 2d6, 1d6)", 3.0)]
    [Arguments("if(1 = 2, 2d6, 1d6)", 1.0)]
    [Arguments("if(1 = 1, 1, 2) + 5", 6.0)]
    [Arguments("2 * if(1 = 1, 3, 5)", 6.0)]
    [Arguments("if(1 = 1, if(2 = 2, 100, 50), 0)", 100.0)]
    [Arguments("if(1 = 2, if(2 = 2, 100, 50), 0)", 0.0)]
    [Arguments("IF(1 = 1, 10, 20)", 10.0)]
    public async Task Should_Return_Correct_Result_From_Conditional_Expression(string input, double expected)
    {
        var result = Evaluate(input);

        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }
    
    [Test]
    [Arguments("if(1d6 >= 4, 2d6, 1d6)", 2.0)]
    public async Task Should_Evaluate_Dice_In_Condition(string input, double expected)
    {
        // With sequential roller: 1d6=1, so condition 1>=4 is false, so 1d6=2
        var result = Evaluate(input);

        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(expected),
            onFailure: error => Assert.Fail(error.Message));
    }
    
    [Test]
    public async Task Should_Return_Correct_Result_From_Conditional_With_Variables()
    {
        var variables = new Dictionary<string, double>
        {
            ["THRESHOLD"] = 15.0,
            ["BONUS"] = 5.0
        };
        
        // 1d20 with sequential roller = 1, 1 >= 15 is false, so result is 1d6 = 2
        var result = EvaluateWithVariables("if(1d20 >= [THRESHOLD], 2d6 + [BONUS], 1d6)", variables);

        await result.SwitchAsync(
            onSuccess: async actual => await Assert.That(actual.Result).IsEqualTo(2.0),
            onFailure: error => Assert.Fail(error.Message));
    }

    private static Result<IRollError, DiceExpressionResult<IRollError, double>> Evaluate(string input, RollerType rollerType = RollerType.Sequential, int[] rolls = default!)
    {
        return rollerType switch
        {
            RollerType.Sequential => DiceExpressionEvaluator<double>.CreateCustom(new SequentialRoller()).Evaluate(input),
            RollerType.Max => DiceExpressionEvaluator<double>.CreateMaximum().Evaluate(input),
            RollerType.Exact => DiceExpressionEvaluator<double>.CreateCustom(new ExactRoller(rolls)).Evaluate(input),
            _ => throw new ArgumentOutOfRangeException(nameof(rollerType), rollerType, null)
        };
    }
    
    private static Result<IRollError, DiceExpressionResult<IRollError, double>> EvaluateWithVariables(string input, IReadOnlyDictionary<string, double> variables, RollerType rollerType = RollerType.Sequential, int[] rolls = default!)
    {
        return rollerType switch
        {
            RollerType.Sequential => DiceExpressionEvaluator<double>.CreateCustom(new SequentialRoller()).Evaluate(input, variables),
            RollerType.Max => DiceExpressionEvaluator<double>.CreateMaximum().Evaluate(input, variables),
            RollerType.Exact => DiceExpressionEvaluator<double>.CreateCustom(new ExactRoller(rolls)).Evaluate(input, variables),
            _ => throw new ArgumentOutOfRangeException(nameof(rollerType), rollerType, null)
        };
    }

    private enum RollerType
    {
        Sequential,
        Max,
        Exact
    }
    
    public record RollsTestData(string Input, List<DiceRoll> Expected, int[] Rolls);
    
#pragma warning disable TUnit0046
    public static IEnumerable<RollsTestData> RollsTestMethodData()
#pragma warning restore TUnit0046
    {
        yield return new RollsTestData("4d6",
            [
                new(6, 1),
                new(6, 2),
                new(6, 3),
                new(6, 4)
            ],
            [
                1, 2, 3, 4
            ]);

        yield return new RollsTestData("(1d4)d6",
            [
                new(4, 2),
                new(6, 4),
                new(6, 6)
            ],
            [
                2,
                4, 6
            ]);

        yield return new RollsTestData("(1d4)d(1d6)",
            [
                new(4, 3),
                new(6, 5),
                new(5, 1),
                new(5, 2),
                new(5, 5)
            ],
            [
                3,
                5,
                1, 2, 5
            ]);

        yield return new RollsTestData("4d6min2max4",
            [
                new(6, 2, DiceModifier.Minimum),
                new(6, 2),
                new(6, 3),
                new(6, 4, DiceModifier.Maximum)
            ],
            [
                1, 2, 3, 5
            ]);

        yield return new RollsTestData("4d6k(1d4)",
            [
                new(6, 4),
                new(6, 3, DiceModifier.Dropped),
                new(6, 3),
                new(6, 5),
                new(4, 3)
            ],
            [
                4, 3, 3, 5,
                3
            ]);

        yield return new RollsTestData("1d6+1d8-1d10*1d12/1d20",
            [
                new(6, 1),
                new(8, 5),
                new(10, 3),
                new(12, 12),
                new(20, 7)
            ],
            [
                1,
                5,
                3,
                12,
                7
            ]);

        yield return new RollsTestData("(1d6)d(1d10)k(1d4)!=(1d8)",
            [
                new(6, 6),
                new(10, 10),
                new(10, 9, DiceModifier.Dropped),
                new(10, 10),
                new(10, 8, DiceModifier.Dropped),
                new(10, 10),
                new(10, 6, DiceModifier.Dropped),
                new(10, 5, DiceModifier.Dropped),
                new(4, 2),
                new(8, 5)
            ],
            [
                6,
                10,
                9, 10, 8, 10, 6, 5,
                2,
                5
            ]);

        yield return new RollsTestData("(1d6)d(1d10)min4!=(1d8)k(1d4)",
            [
                new(6, 6),
                new(10, 10),
                new(10, 9, DiceModifier.Dropped),
                new(10, 10),
                new(10, 8, DiceModifier.Dropped),
                new(10, 10),
                new(10, 4, DiceModifier.Minimum | DiceModifier.Dropped),
                new(10, 5, DiceModifier.Exploded | DiceModifier.Dropped),
                new(10, 1, DiceModifier.Dropped),
                new(8, 5),
                new(4, 2)
            ],
            [
                6,
                10,
                9, 10, 8, 10, 3, 5,
                5,
                1,
                2
            ]);

        yield return new RollsTestData("4d6ro",
            [
                new(6, 6, DiceModifier.Rerolled),
                new(6, 2),
                new(6, 3),
                new(6, 5)
            ],
            [
                1, 2, 3, 5,
                6
            ]);

        yield return new RollsTestData("4d6r",
            [
                new(6, 2, DiceModifier.Rerolled),
                new(6, 2),
                new(6, 3),
                new(6, 5)
            ],
            [
                1, 2, 3, 5,
                1, 1, 2
            ]);

        yield return new RollsTestData("4d6r<3",
            [
                new(6, 3, DiceModifier.Rerolled),
                new(6, 4, DiceModifier.Rerolled),
                new(6, 3),
                new(6, 5)
            ],
            [
                1, 2, 3, 5,
                1, 1, 3,
                2, 1, 4
            ]);

        yield return new RollsTestData("4d6k3r",
            [
                new(6, 1, DiceModifier.Dropped),
                new(6, 2),
                new(6, 3),
                new(6, 5)
            ],
            [
                1, 2, 3, 5
            ]);

        yield return new RollsTestData("4d6k3r",
            [
                new(6, 1, DiceModifier.Dropped),
                new(6, 2),
                new(6, 3),
                new(6, 5)
            ],
            [
                1, 2, 3, 5
            ]);
    }
}