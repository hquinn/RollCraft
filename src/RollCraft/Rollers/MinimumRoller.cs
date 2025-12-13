namespace RollCraft.Rollers;

/// <summary>
/// A roller that always returns the minimum possible value (1) for any die.
/// </summary>
/// <remarks>
/// Useful for testing worst-case scenarios or calculating minimum possible outcomes.
/// </remarks>
public class MinimumRoller : IRoller
{
    /// <inheritdoc />
    public int RollDice(int dieSize)
    {
        return 1;
    }
}