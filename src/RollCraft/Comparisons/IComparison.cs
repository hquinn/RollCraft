using MonadCraft;

namespace RollCraft.Comparisons;

internal interface IComparison
{
    Result<IRollError, (bool Success, List<DiceRoll> Rolls)> RollEquals(IRoller roller, DiceRoll roll);
    void Reset();
}