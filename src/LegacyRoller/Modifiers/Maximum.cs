using LitePrimitives;

namespace LegacyRoller.Modifiers;

internal sealed class Maximum : IModifier
{
    internal Maximum(DiceExpression maximum)
    {
        MaximumValue = maximum;
    }

    internal DiceExpression MaximumValue { get; }

    public Result<Unit> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {        
        var maximumValue = MaximumValue.Evaluate(roller);

        if (maximumValue.IsFailure)
        {
            return maximumValue.Map<Unit>(_ => default);
        }
        
        var maximum = (long)maximumValue.Value!.Result;
        
        // Hack to check if the maximum is a valid integer
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (maximumValue.Value.Result != maximum)
        {
            return Result<Unit>.Failure(new EvaluatorError("MaximumError", "Maximum must be an integer!"));
        }
        
        // A normal dice roll cannot have a minimum value less than 1
        if (maximum < 1)
        {
            return Result<Unit>.Failure(new EvaluatorError("MaximumError", "Cannot have a maximum value less than 1!"));
        }

        // A normal dice roll cannot have a maximum value more than the dice side count
        if (maximum > diceRolls[0].Sides)
        {
            return Result<Unit>.Failure(new EvaluatorError("MaximumError", "Cannot have a maximum value greater than the dice side count!"));
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
        
        return Result<Unit>.Success(default);
    }
    
    public override string ToString()
    {
        return $"MAXIMUM={MaximumValue}";
    }
}