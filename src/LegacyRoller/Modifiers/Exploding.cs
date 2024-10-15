using LegacyRoller.Comparisons;
using LitePrimitives;

namespace LegacyRoller.Modifiers;

internal sealed class Exploding : IModifier
{
    private const int MaxIterationsAboveOriginalCount = 1000;
    
    internal Exploding(IComparison comparison)
    {
        Comparison = comparison;
    }

    internal IComparison Comparison { get; }

    public Result<Unit> Modify(IRandom random, List<DiceRoll> diceRolls)
    {
        var originalDiceRollCount = diceRolls.Count;
        
        for (var index = 0; index < diceRolls.Count; index++)
        {
            if (diceRolls.Count - originalDiceRollCount > MaxIterationsAboveOriginalCount)
            {
                break;
            }
            
            var diceRoll = diceRolls[index];
            var comparisonResult = Comparison.RollEquals(random, diceRoll);

            if (comparisonResult.IsFailure)
            {
                return comparisonResult.Map<Unit>(_ => default);
            }

            if (!comparisonResult.Value)
            {
                continue;
            }
            
            diceRoll.Modifier |= DiceModifier.Exploded;

            var roll = random.RollDice(diceRoll.Sides);
            var newDiceRoll = new DiceRoll(diceRoll.Sides, roll);
            diceRolls.Add(newDiceRoll);
        }

        Comparison.Reset();
        
        return Result<Unit>.Success(default);
    }
    
    public override string ToString()
    {
        return $"EXPLODING={Comparison}";
    }
}