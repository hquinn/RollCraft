namespace RollCraft.Rollers;

/// <summary>
/// A roller that always returns the maximum possible value for any die.
/// </summary>
/// <remarks>
/// Useful for testing best-case scenarios or calculating maximum possible outcomes.
/// </remarks>
public class MaximumRoller : IRoller
{
    /// <inheritdoc />
    public int RollDice(int dieSize)
    {
        return dieSize;
    }
}