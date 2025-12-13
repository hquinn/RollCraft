namespace RollCraft.Rollers;

/// <summary>
/// A roller that always returns the average value for any die (rounded up).
/// </summary>
/// <remarks>
/// <para>
/// The average is calculated as <c>(dieSize / 2) + 1</c>. For example:
/// </para>
/// <list type="bullet">
/// <item><description>d4 returns 3</description></item>
/// <item><description>d6 returns 4</description></item>
/// <item><description>d8 returns 5</description></item>
/// <item><description>d10 returns 6</description></item>
/// <item><description>d12 returns 7</description></item>
/// <item><description>d20 returns 11</description></item>
/// </list>
/// <para>
/// Useful for calculating expected average outcomes.
/// </para>
/// </remarks>
public class FixedAverageRoller : IRoller
{
    /// <inheritdoc />
    public int RollDice(int dieSize)
    {
        return dieSize / 2 + 1;
    }
}