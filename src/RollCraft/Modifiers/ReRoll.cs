using MonadCraft;
using RollCraft.Comparisons;
using RollCraft.Helpers;

namespace RollCraft.Modifiers;

internal sealed class ReRoll : IModifier
{
    private const int MaxIterationsPerDice = 1000;
    
    internal ReRoll(IComparison comparison, bool reRollOnce)
    {
        Comparison = comparison;
        ReRollOnce = reRollOnce;
    }

    internal IComparison Comparison { get; }
    internal bool ReRollOnce { get; }

    public Result<IRollError, List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var iterationMax = ReRollOnce ? 1 : MaxIterationsPerDice;
        var firstComparison = true;
        List<DiceRoll> comparisonRolls = null!;
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            var diceRoll = diceRolls[index];
                
            if (diceRoll.HasDiceDropped())
            {
                continue;
            }
            
            for (var iteration = 0; iteration < iterationMax; iteration++)
            {
                
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
                    break;
                }
                
                diceRoll.Modifier |= DiceModifier.Rerolled;

                var roll = roller.RollDice(diceRoll.Sides);
                diceRoll.Roll = roll;
                
                comparisonResult = Comparison.RollEquals(roller, diceRoll);

                if (!comparisonResult.Value.Success)
                {
                    break;
                }
            }
        }

        Comparison.Reset();
        
        return Result<IRollError, List<DiceRoll>>.Success(comparisonRolls!);
    }

    public override string ToString()
    {
        return $"REROLL{(ReRollOnce ? "ONCE" : "")}={Comparison}";
    }
}