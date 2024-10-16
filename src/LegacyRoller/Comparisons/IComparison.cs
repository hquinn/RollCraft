using LitePrimitives;

namespace LegacyRoller.Comparisons;

internal interface IComparison
{
    Result<(bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll);
    void Reset();
}