using LitePrimitives;
using RollCraft.Full.Modifiers;

namespace RollCraft.Full.Nodes;

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

    internal override Result<(double Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {        
        var countOfDiceResult = CountOfDice.EvaluateNode(roller);
        if (countOfDiceResult.IsFailure)
        {
            return countOfDiceResult;
        }

        var countOfSidesResult = CountOfSides.EvaluateNode(roller);
        if (countOfSidesResult.IsFailure)
        {
            return countOfSidesResult;
        }
        
        var diceCount = (long)countOfDiceResult.Value.Result;
        
        // Hack to check if the dice count is a valid integer
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (countOfDiceResult.Value.Result != diceCount)
        {
            return Result<(double Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DiceError", "Dice count must be an integer!"));
        }

        if (diceCount == 0)
        {
            return Result<(double Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DiceError", "Dice count must not be 0!"));
        }
        
        var sides = (long)countOfSidesResult.Value.Result;
        
        // Hack to check if the dice sides is a valid integer
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (countOfSidesResult.Value.Result != sides)
        {
            return Result<(double Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DiceError", "Dice sides must be an integer!"));
        }

        if (sides <= 0)
        {
            return Result<(double Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DiceError", "Dice sides must not be 0 or less!"));
        }

        var diceRolls = new List<DiceRoll>();
        
        var sidesInt = (int)sides;
        var countIsNegative = diceCount < 0;
        var countInt = (int)Math.Abs(diceCount);
        
        for (var i = 0; i < countInt; i++)
        {
            diceRolls.Add(new(sidesInt, roller.RollDice(sidesInt)));
        }

        var modifierRolls = new List<DiceRoll>();
        foreach (var modifier in Modifiers)
        {
            var result = modifier.Modify(roller, diceRolls);
            if (result.IsFailure)
            {
                return result.Map<(double Result, List<DiceRoll> Rolls)>(_ => default);
            }
            
            modifierRolls.AddRange(result.Value!);
        }

        var total = 0.0;
        foreach (var diceRoll in diceRolls)
        {
            if ((diceRoll.Modifier & DiceModifier.Dropped) != 0)
            {
                continue;
            }
            
            total += diceRoll.Roll;
        }
        
        // This should preserve the ordering of when the dice was rolled
        countOfDiceResult.Value.Rolls.AddRange(countOfSidesResult.Value!.Rolls);
        countOfDiceResult.Value.Rolls.AddRange(diceRolls);
        countOfDiceResult.Value.Rolls.AddRange(modifierRolls);
        
        return Result<(double Result, List<DiceRoll> Rolls)>.Success((countIsNegative ? -total : total, countOfDiceResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        var modifiers = string.Join(", ", Modifiers);
        return $"DICE({CountOfDice}, {CountOfSides}{(string.IsNullOrEmpty(modifiers) ? "" : $", {modifiers}")})";
    }
}