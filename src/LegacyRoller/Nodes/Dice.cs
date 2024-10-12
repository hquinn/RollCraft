using LegacyRoller.Modifiers;
using LitePrimitives;

namespace LegacyRoller.Nodes;

internal sealed class Dice : DiceExpression
{
    internal DiceExpression CountOfDice { get; }
    internal DiceExpression CountOfSides { get; }
    
    internal List<IModifier> Modifiers { get; } = new();

    internal Dice(DiceExpression countOfDice, DiceExpression countOfSides)
    {
        CountOfDice = countOfDice;
        CountOfSides = countOfSides;
    }

    internal override Result<double> EvaluateNode(IRandom random)
    {        
        var countOfDiceResult = CountOfDice.EvaluateNode(random);
        if (countOfDiceResult.IsFailure)
        {
            return countOfDiceResult;
        }

        var countOfSidesResult = CountOfSides.EvaluateNode(random);
        if (countOfSidesResult.IsFailure)
        {
            return countOfSidesResult;
        }
        
        var diceCount = (long)countOfDiceResult.Value;
        
        // Hack to check if the dice count is a valid integer
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (countOfDiceResult.Value != diceCount)
        {
            return Result<double>.Failure(new EvaluatorError("DiceError", "Dice count must be an integer!"));
        }

        if (diceCount == 0)
        {
            return Result<double>.Failure(new EvaluatorError("DiceError", "Dice count must not be 0!"));
        }
        
        var sides = (long)countOfSidesResult.Value;
        
        // Hack to check if the dice sides is a valid integer
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (countOfSidesResult.Value != sides)
        {
            return Result<double>.Failure(new EvaluatorError("DiceError", "Dice sides must be an integer!"));
        }

        if (sides <= 0)
        {
            return Result<double>.Failure(new EvaluatorError("DiceError", "Dice sides must not be 0 or less!"));
        }

        var total = 0.0;
        var sidesInt = (int)sides;
        var countIsNegative = diceCount < 0;
        var countInt = (int)Math.Abs(diceCount);
        
        for (var i = 0; i < countInt; i++)
        {
            total += random.RollDice(sidesInt);
        }
        
        return Result<double>.Success(countIsNegative ? -total : total);
    }
    
    public override string ToString()
    {
        var modifiers = string.Join(", ", Modifiers);
        return $"DICE({CountOfDice}, {CountOfSides}{(string.IsNullOrEmpty(modifiers) ? "" : $", {modifiers}")})";
    }
}