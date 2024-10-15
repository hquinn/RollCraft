using LitePrimitives;

namespace LegacyRoller.Comparisons;

internal interface IComparison
{
    Result<bool> RollEquals(IRandom random, DiceRoll roll);
    void Reset();
}