using LitePrimitives;

namespace RollCraft.Simple.Comparisons;

internal interface IComparison
{
    Result<(bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll);
    void Reset();
}