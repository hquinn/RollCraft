using LitePrimitives;
using RollCraft.Full.Comparisons;

namespace RollCraft.Full.Modifiers;

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

    public Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var iterationMax = ReRollOnce ? 1 : MaxIterationsPerDice;
        var firstComparison = true;
        List<DiceRoll> comparisonRolls = null!;
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            for (var iteration = 0; iteration < iterationMax; iteration++)
            {
                var diceRoll = diceRolls[index];
                var comparisonResult = Comparison.RollEquals(roller, diceRoll);

                if (comparisonResult.IsFailure)
                {
                    return comparisonResult.Map<List<DiceRoll>>(_ => default!);
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
        
        return Result<List<DiceRoll>>.Success(comparisonRolls!);
    }

    public override string ToString()
    {
        return $"REROLL{(ReRollOnce ? "ONCE" : "")}={Comparison}";
    }
}