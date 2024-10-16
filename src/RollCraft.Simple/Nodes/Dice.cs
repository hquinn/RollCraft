using LitePrimitives;
using RollCraft.Simple.Modifiers;

namespace RollCraft.Simple.Nodes;

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

    internal override Result<(int Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
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
        
        var diceCount = countOfDiceResult.Value.Result;

        if (diceCount == 0)
        {
            return Result<(int Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DiceError", "Dice count must not be 0!"));
        }
        
        var sides = countOfSidesResult.Value.Result;

        if (sides <= 0)
        {
            return Result<(int Result, List<DiceRoll> Rolls)>.Failure(new EvaluatorError("DiceError", "Dice sides must not be 0 or less!"));
        }

        var diceRolls = new List<DiceRoll>();
        
        var countIsNegative = diceCount < 0;
        var countAbs = Math.Abs(diceCount);
        
        for (var i = 0; i < countAbs; i++)
        {
            diceRolls.Add(new(sides, roller.RollDice(sides)));
        }

        var modifierRolls = new List<DiceRoll>();
        foreach (var modifier in Modifiers)
        {
            var result = modifier.Modify(roller, diceRolls);
            if (result.IsFailure)
            {
                return result.Map<(int Result, List<DiceRoll> Rolls)>(_ => default);
            }
            
            modifierRolls.AddRange(result.Value!);
        }

        var total = 0;
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
        
        return Result<(int Result, List<DiceRoll> Rolls)>.Success((countIsNegative ? -total : total, countOfDiceResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        var modifiers = string.Join(", ", Modifiers);
        return $"DICE({CountOfDice}, {CountOfSides}{(string.IsNullOrEmpty(modifiers) ? "" : $", {modifiers}")})";
    }
}