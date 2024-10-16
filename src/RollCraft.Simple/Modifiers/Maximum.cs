using LitePrimitives;

namespace RollCraft.Simple.Modifiers;

internal sealed class Maximum : IModifier
{
    internal Maximum(DiceExpression maximum)
    {
        MaximumValue = maximum;
    }

    internal DiceExpression MaximumValue { get; }

    public Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {        
        var maximumValue = MaximumValue.Evaluate(roller);

        if (maximumValue.IsFailure)
        {
            return maximumValue.Map<List<DiceRoll>>(_ => default!);
        }
        
        var maximum = maximumValue.Value!.Result;
        
        // A normal dice roll cannot have a minimum value less than 1
        if (maximum < 1)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("MaximumError", "Cannot have a maximum value less than 1!"));
        }

        // A normal dice roll cannot have a maximum value more than the dice side count
        if (maximum > diceRolls[0].Sides)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("MaximumError", "Cannot have a maximum value greater than the dice side count!"));
        }

        var maximumInt = (int)maximum;
        
        foreach (var diceRoll in diceRolls)
        {
            if (diceRoll.Roll > maximum)
            {
                diceRoll.Roll = maximumInt;
                diceRoll.Modifier |= DiceModifier.Maximum;
            }
        }
        
        return Result<List<DiceRoll>>.Success(maximumValue.Value.Rolls);
    }
    
    public override string ToString()
    {
        return $"MAXIMUM={MaximumValue}";
    }
}