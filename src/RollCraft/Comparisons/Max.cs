using MonadCraft;

namespace RollCraft.Comparisons;

public class Max : IComparison
{
    public Result<IRollError, (bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return Result<IRollError, (bool Success, List<DiceRoll> Rolls)>.Success((roll.Roll == roll.Sides, []));
    }

    public void Reset()
    {
    }
    
    public override string ToString()
    {
        return "MAX";
    }
}