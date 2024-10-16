using LitePrimitives;

namespace RollCraft.Simple.Modifiers;

internal sealed class Minimum : IModifier
{
    internal Minimum(DiceExpression minimum)
    {
        MinimumValue = minimum;
    }

    internal DiceExpression MinimumValue { get; }

    public Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var minimumValue = MinimumValue.Evaluate(roller);

        if (minimumValue.IsFailure)
        {
            return minimumValue.Map<List<DiceRoll>>(_ => default!);
        }
        
        var minimum = minimumValue.Value!.Result;
        
        // A normal dice roll cannot have a minimum value less than 1
        if (minimum < 1)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("MinimumError", "Cannot have a minimum value less than 1!"));
        }

        // A normal dice roll cannot have a maximum value more than the dice side count
        if (minimum > diceRolls[0].Sides)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("MinimumError", "Cannot have a minimum value greater than the dice side count!"));
        }
        
        var minimumInt = (int)minimum;
        
        foreach (var diceRoll in diceRolls)
        {
            if (diceRoll.Roll < minimum)
            {
                diceRoll.Roll = minimumInt;
                diceRoll.Modifier |= DiceModifier.Minimum;
            }
        }
        
        return Result<List<DiceRoll>>.Success(minimumValue.Value.Rolls);
    }
    
    public override string ToString()
    {
        return $"MINIMUM={MinimumValue}";
    }
}