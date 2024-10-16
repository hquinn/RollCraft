using LitePrimitives;

namespace RollCraft.Simple.Comparisons;

public class Min : IComparison
{
    public Result<(bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll)
    {
        return Result<(bool Success, List<DiceRoll> Rolls)>.Success((roll.Roll == 1, []));
    }

    public void Reset()
    {
    }
    
    public override string ToString()
    {
        return "MIN";
    }
}