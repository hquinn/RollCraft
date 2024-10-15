using LitePrimitives;

namespace LegacyRoller.Comparisons;

public class Max : IComparison
{
    public Result<bool> RollEquals(IRoller roller, DiceRoll roll)
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        return Result<bool>.Success(roll.Roll == roll.Sides);
    }

    public void Reset()
    {
    }
    
    public override string ToString()
    {
        return "MAX";
    }
}