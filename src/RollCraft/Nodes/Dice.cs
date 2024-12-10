using System.Numerics;
using LitePrimitives;
using RollCraft.Helpers;
using RollCraft.Modifiers;

namespace RollCraft.Nodes;

internal sealed class Dice<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal DiceExpression<TNumber> CountOfDice { get; }
    internal DiceExpression<TNumber> CountOfSides { get; }
    
    internal List<IModifier> Modifiers { get; } = new(5);

    internal Dice(DiceExpression<TNumber> countOfDice, DiceExpression<TNumber> countOfSides)
    {
        CountOfDice = countOfDice;
        CountOfSides = countOfSides;
    }

    internal override Result<(TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
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
        
        if (!TNumber.IsInteger(countOfDiceResult.Value.Result))
        {
            return Error.Default("Evaluator.DiceError", "Dice count must be an integer!");
        }

        var diceCount = int.CreateSaturating(countOfDiceResult.Value.Result);

        if (diceCount == 0)
        {
            return Error.Default("Evaluator.DiceError", "Dice count must not be 0!");
        }
        
        if (!TNumber.IsInteger(countOfSidesResult.Value.Result))
        {
            return Error.Default("Evaluator.DiceError", "Dice sides must be an integer!");
        }

        var sides = int.CreateSaturating(countOfSidesResult.Value.Result);
        
        if (sides < 1)
        {
            return Error.Default("Evaluator.DiceError", "Dice sides must not be 0 or less!");
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
                return Result<(TNumber Result, List<DiceRoll> Rolls)>.Failure(result.Error!);
            }
            
            modifierRolls.AddRange(result.Value!);
        }

        var total = TNumber.Zero;
        foreach (var diceRoll in diceRolls)
        {
            if (diceRoll.HasDiceDropped())
            {
                continue;
            }
            
            total += TNumber.CreateSaturating(diceRoll.Roll);
        }
        
        // This should preserve the ordering of when the dice was rolled
        countOfDiceResult.Value.Rolls.AddRange(countOfSidesResult.Value!.Rolls);
        countOfDiceResult.Value.Rolls.AddRange(diceRolls);
        countOfDiceResult.Value.Rolls.AddRange(modifierRolls);
        
        return Result<(TNumber Result, List<DiceRoll> Rolls)>.Success((countIsNegative ? -total : total, countOfDiceResult.Value.Rolls));
    }
    
    public override string ToString()
    {
        var modifiers = string.Join(", ", Modifiers);
        return $"DICE({CountOfDice}, {CountOfSides}{(string.IsNullOrEmpty(modifiers) ? "" : $", {modifiers}")})";
    }
}