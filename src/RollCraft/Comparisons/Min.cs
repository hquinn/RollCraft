using MonadCraft;

namespace RollCraft.Comparisons;

public class Min : IComparison
{
    public Result<IRollError, (bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll)
    {
        return Result<IRollError, (bool Success, List<DiceRoll> Rolls)>.Success((roll.Roll == 1, []));
    }

    public void Reset()
    {
    }
    
    public override string ToString()
    {
        return "MIN";
    }
}