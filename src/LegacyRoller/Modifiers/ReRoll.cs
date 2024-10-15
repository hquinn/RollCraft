using LegacyRoller.Comparisons;
using LitePrimitives;

namespace LegacyRoller.Modifiers;

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

    public Result<Unit> Modify(IRoller roller, List<DiceRoll> diceRolls)
    {
        var iterationMax = ReRollOnce ? 1 : MaxIterationsPerDice;
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            for (var iteration = 0; iteration < iterationMax; iteration++)
            {
                var diceRoll = diceRolls[index];
                var comparisonResult = Comparison.RollEquals(roller, diceRoll);

                if (comparisonResult.IsFailure)
                {
                    return comparisonResult.Map<Unit>(_ => default);
                }

                if (!comparisonResult.Value)
                {
                    break;
                }
                
                diceRoll.Modifier |= DiceModifier.Rerolled;

                var roll = roller.RollDice(diceRoll.Sides);
                diceRoll.Roll = roll;
                
                comparisonResult = Comparison.RollEquals(roller, diceRoll);

                if (!comparisonResult.Value)
                {
                    break;
                }
            }
        }

        Comparison.Reset();
        
        return Result<Unit>.Success(default);
    }

    public override string ToString()
    {
        return $"REROLL{(ReRollOnce ? "ONCE" : "")}={Comparison}";
    }
}