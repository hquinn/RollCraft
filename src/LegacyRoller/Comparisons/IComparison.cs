using LitePrimitives;

namespace LegacyRoller.Comparisons;

internal interface IComparison
{
    Result<bool> RollEquals(IRoller roller, DiceRoll roll);
    void Reset();
}