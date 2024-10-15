using LitePrimitives;

namespace LegacyRoller.Comparisons;

public class Min : IComparison
{
    public Result<bool> RollEquals(IRoller roller, DiceRoll roll)
    {
        return Result<bool>.Success(roll.Roll == 1);
    }

    public void Reset()
    {
    }
    
    public override string ToString()
    {
        return "MIN";
    }
}