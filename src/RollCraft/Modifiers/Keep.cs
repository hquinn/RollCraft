using System.Numerics;
using LitePrimitives;

namespace RollCraft.Modifiers;

internal sealed class Keep<TNumber> : IModifier
    where TNumber : INumber<TNumber>
{
    private readonly bool _keepHighest;
    
    internal Keep(DiceExpression<TNumber> countValue, bool keepHighest)
    {
        CountValue = countValue;
        _keepHighest = keepHighest;
    }

    internal DiceExpression<TNumber> CountValue { get; }

    public Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var countValue = CountValue.Evaluate(roller);

        if (countValue.IsFailure)
        {
            return Result<List<DiceRoll>>.Failure(countValue.Errors!);
        }
        
        if (!TNumber.IsInteger(countValue.Value!.Result))
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("KeepError", "Keep must be an integer!"));
        }

        var count = int.CreateSaturating(countValue.Value!.Result);
        
        // Cannot keep negative numbers
        if (count <= -1)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("KeepError", "Keep must be zero or more!"));
        }
        
        if (count > diceRolls.Count)
        {
            return Result<List<DiceRoll>>.Failure(new EvaluatorError("KeepError", "Keep must be less or equal than number of dice rolled!"));
        }

        if (_keepHighest)
        {
            KeepHighest(diceRolls, count);
        }
        else
        {
            KeepLowest(diceRolls, count);
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
        
        DropRolls(diceRolls, keptRolls);
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
        
        DropRolls(diceRolls, keptRolls);
    }

    private static void DropRolls(List<DiceRoll> diceRolls, Span<int> keptRolls)
    {
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