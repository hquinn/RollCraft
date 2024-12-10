using System.Numerics;
using LitePrimitives;

namespace RollCraft.Modifiers;

internal sealed class Minimum<TNumber> : IModifier
    where TNumber : INumber<TNumber>
{
    internal Minimum(DiceExpression<TNumber> minimum)
    {
        MinimumValue = minimum;
    }

    internal DiceExpression<TNumber> MinimumValue { get; }

    public Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var minimumValue = MinimumValue.Evaluate(roller);

        if (minimumValue.IsFailure)
        {
            return Result<List<DiceRoll>>.Failure(minimumValue.Error!);
        }
        
        if (!TNumber.IsInteger(minimumValue.Value!.Result))
        {
            return Error.Default("Evaluator.MinimumError", "Minimum must be an integer!");
        }

        var minimum = int.CreateSaturating(minimumValue.Value!.Result);
        
        // A normal dice roll cannot have a minimum value less than 1
        if (minimum < 1)
        {
            return Error.Default("Evaluator.MinimumError", "Cannot have a minimum value less than 1!");
        }

        // A normal dice roll cannot have a maximum value more than the dice side count
        if (minimum > diceRolls[0].Sides)
        {
            return Error.Default("Evaluator.MinimumError", "Cannot have a minimum value greater than the dice side count!");
        }

        foreach (var diceRoll in diceRolls)
        {
            if (diceRoll.Roll < minimum)
            {
                diceRoll.Roll = minimum;
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