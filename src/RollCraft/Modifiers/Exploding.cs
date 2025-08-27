using MonadCraft;
using RollCraft.Comparisons;
using RollCraft.Helpers;

namespace RollCraft.Modifiers;

internal sealed class Exploding : IModifier
{
    private const int MaxIterationsAboveOriginalCount = 1000;
    
    internal Exploding(IComparison comparison)
    {
        Comparison = comparison;
    }

    internal IComparison Comparison { get; }

    public Result<IRollError, List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var originalDiceRollCount = diceRolls.Count;
        var firstComparison = true;
        List<DiceRoll> comparisonRolls = null!;
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            if (diceRolls.Count - originalDiceRollCount > MaxIterationsAboveOriginalCount)
            {
                break;
            }
            
            var diceRoll = diceRolls[index];
            
            if (diceRoll.HasDiceDropped() || diceRoll.HasDiceExploded())
            {
                continue;
            }
            
            var comparisonResult = Comparison.RollEquals(roller, diceRoll);

            if (comparisonResult.IsFailure)
            {
                return Result<IRollError, List<DiceRoll>>.Failure(comparisonResult.Error);
            }
            
            if (firstComparison)
            {
                firstComparison = false;
                comparisonRolls = comparisonResult.Value.Rolls;
            }

            if (!comparisonResult.Value.Success)
            {
                continue;
            }
            
            diceRoll.Modifier |= DiceModifier.Exploded;

            var roll = roller.RollDice(diceRoll.Sides);
            var newDiceRoll = new DiceRoll(diceRoll.Sides, roll);
            diceRolls.Add(newDiceRoll);
        }

        Comparison.Reset();
        
        return Result<IRollError, List<DiceRoll>>.Success(comparisonRolls!);
    }
    
    public override string ToString()
    {
        return $"EXPLODING={Comparison}";
    }
}