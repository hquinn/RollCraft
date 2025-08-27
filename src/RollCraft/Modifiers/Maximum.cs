using System.Numerics;
using MonadCraft;

namespace RollCraft.Modifiers;

internal sealed class Maximum<TNumber> : IModifier
    where TNumber : INumber<TNumber>
{
    internal Maximum(DiceExpression<TNumber> maximum)
    {
        MaximumValue = maximum;
    }

    internal DiceExpression<TNumber> MaximumValue { get; }

    public Result<IRollError, List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {        
        var maximumValue = MaximumValue.Evaluate(roller);

        if (maximumValue.IsFailure)
        {
            return Result<IRollError, List<DiceRoll>>.Failure(maximumValue.Error);
        }
        
        if (!TNumber.IsInteger(maximumValue.Value.Result))
        {
            return new EvaluatorError("Evaluator.MaximumError", "Maximum must be an integer!");
        }

        var maximum = int.CreateSaturating(maximumValue.Value.Result);
        
        // A normal dice roll cannot have a minimum value less than 1
        if (maximum < 1)
        {
            return new EvaluatorError("Evaluator.MaximumError", "Cannot have a maximum value less than 1!");
        }

        // A normal dice roll cannot have a maximum value more than the dice side count
        if (maximum > diceRolls[0].Sides)
        {
            return new EvaluatorError("Evaluator.MaximumError", "Cannot have a maximum value greater than the dice side count!");
        }

        foreach (var diceRoll in diceRolls)
        {
            if (diceRoll.Roll > maximum)
            {
                diceRoll.Roll = maximum;
                diceRoll.Modifier |= DiceModifier.Maximum;
            }
        }
        
        return Result<IRollError, List<DiceRoll>>.Success(maximumValue.Value.Rolls);
    }
    
    public override string ToString()
    {
        return $"MAXIMUM={MaximumValue}";
    }
}