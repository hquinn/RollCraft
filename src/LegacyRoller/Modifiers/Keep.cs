using LitePrimitives;

namespace LegacyRoller.Modifiers;

internal sealed class Keep : IModifier
{
    private readonly bool _keepHighest;
    
    internal Keep(DiceExpression countValue, bool keepHighest)
    {
        CountValue = countValue;
        _keepHighest = keepHighest;
    }

    internal DiceExpression CountValue { get; }

    public Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var countValue = CountValue.Evaluate(roller);

        if (countValue.IsFailure)
        {
            return countValue.Map<List<DiceRoll>>(_ => default!);
        }

        var count = (long)countValue.Value!.Result;
        
        // Hack to check if the minimum is a valid integer
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (countValue.Value.Result != count)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("KeepError", "Keep must be an integer!"));
        }
        
        // Cannot keep negative numbers
        if (count <= -1)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("KeepError", "Keep must be zero or more!"));
        }
        
        if (count > diceRolls.Count)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("KeepError", "Keep must be less or equal than number of dice rolled!"));
        }
        
        var countInt = (int)count;

        if (_keepHighest)
        {
            KeepHighest(diceRolls, countInt);
        }
        else
        {
            KeepLowest(diceRolls, countInt);
        }

        return Result<List<DiceRoll>>.Success(countValue.Value.Rolls);
    }

    private static void KeepHighest(List<DiceRoll> diceRolls, int count)
    {
        int keptCount = 0;
        
        Span<int> keptRolls = stackalloc int[count];

        for (var index = 0; index < diceRolls.Count; index++)
        {
            var diceRoll = diceRolls[index];
            var roll = diceRoll.Roll;
            
            if (keptCount < count)
            {
                keptRolls[keptCount] = index;
                keptCount++;
                continue;
            }

            for (var keptIndex = 0; keptIndex < keptCount; keptIndex++)
            {
                var keptRoll = diceRolls[keptRolls[keptIndex]];
                
                if (roll > keptRoll.Roll)
                {
                    keptRolls[keptIndex] = index;
                    break;
                }
            }
        }
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            if (keptRolls.Contains(index))
            {
                continue;
            }
            
            diceRolls[index].Modifier |= DiceModifier.Dropped;
        }
    }

    private static void KeepLowest(List<DiceRoll> diceRolls, int count)
    {
        int keptCount = 0;
        
        Span<int> keptRolls = stackalloc int[count];

        for (var index = 0; index < diceRolls.Count; index++)
        {
            var diceRoll = diceRolls[index];
            var roll = diceRoll.Roll;
            
            if (keptCount < count)
            {
                keptRolls[keptCount] = index;
                keptCount++;
                continue;
            }

            for (var keptIndex = 0; keptIndex < keptCount; keptIndex++)
            {
                var keptRoll = diceRolls[keptRolls[keptIndex]];
                
                if (roll < keptRoll.Roll)
                {
                    keptRolls[keptIndex] = index;
                    break;
                }
            }
        }
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            if (keptRolls.Contains(index))
            {
                continue;
            }
            
            diceRolls[index].Modifier |= DiceModifier.Dropped;
        }
    }

    public override string ToString()
    {
        if (!_keepHighest)
        {
            return $"KEEPLOWEST={CountValue}";
        }
        
        return $"KEEPHIGHEST={CountValue}";
    }
}