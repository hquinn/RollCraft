using System.Numerics;
using MonadCraft;

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

    public Result<IRollError, List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var countValue = CountValue.Evaluate(roller);

        if (countValue.IsFailure)
        {
            return Result<IRollError, List<DiceRoll>>.Failure(countValue.Error);
        }
        
        if (!TNumber.IsInteger(countValue.Value.Result))
        {
            return new EvaluatorError("Evaluator.KeepError", "Keep must be an integer!");
        }

        var count = int.CreateSaturating(countValue.Value.Result);
        
        // Cannot keep negative numbers
        if (count <= -1)
        {
            return new EvaluatorError("Evaluator.KeepError", "Keep must be zero or more!");
        }
        
        if (count > diceRolls.Count)
        {
            return new EvaluatorError("Evaluator.KeepError", "Keep must be less or equal than number of dice rolled!");
        }

        KeepRolls(diceRolls, count, _keepHighest);

        return Result<IRollError, List<DiceRoll>>.Success(countValue.Value.Rolls);
    }

    private static void KeepRolls(List<DiceRoll> diceRolls, int count, bool keepHighest)
    {
        var keptCount = 0;
        Span<int> keptRolls = stackalloc int[count];

        for (var index = 0; index < diceRolls.Count; index++)
        {
            var roll = diceRolls[index].Roll;

            if (keptCount < count)
            {
                keptRolls[keptCount++] = index;
            }
            else
            {
                ReplaceKeptRolls(diceRolls, keepHighest, keptRolls, keptCount, roll, index);
            }
        }

        DropRolls(diceRolls, keptRolls.Slice(0, keptCount));
    }

    private static void ReplaceKeptRolls(List<DiceRoll> diceRolls, bool keepHighest, Span<int> keptRolls, int keptCount, int roll,
        int index)
    {
        var targetValue = diceRolls[keptRolls[0]].Roll;
        var targetIndex = 0;

        // Find the kept roll to potentially replace
        for (var keptIndex = 1; keptIndex < keptCount; keptIndex++)
        {
            var keptRollValue = diceRolls[keptRolls[keptIndex]].Roll;

            var condition = keepHighest
                ? keptRollValue < targetValue
                : keptRollValue > targetValue;

            if (condition)
            {
                targetValue = keptRollValue;
                targetIndex = keptIndex;
            }
        }

        // Decide whether to replace the kept roll
        var replaceCondition = keepHighest
            ? roll > targetValue
            : roll < targetValue;

        if (replaceCondition)
        {
            keptRolls[targetIndex] = index;
        }
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