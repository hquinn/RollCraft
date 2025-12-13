namespace RollCraft;

/// <summary>
/// Interface for custom dice rolling implementations.
/// </summary>
/// <remarks>
/// <para>
/// Implement this interface to provide custom dice rolling behavior. Built-in implementations include:
/// </para>
/// <list type="bullet">
/// <item><description>Random roller - Uses a cryptographically secure random number generator</description></item>
/// <item><description>Seeded random roller - Uses a seeded RNG for reproducible results</description></item>
/// <item><description><see cref="Rollers.MinimumRoller"/> - Always rolls 1</description></item>
/// <item><description><see cref="Rollers.MaximumRoller"/> - Always rolls the maximum value</description></item>
/// <item><description><see cref="Rollers.FixedAverageRoller"/> - Always rolls the average value (rounded up)</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// public class LoadedDiceRoller : IRoller
/// {
///     public int RollDice(int dieSize)
///     {
///         // Always roll 6 on a d6, max on other dice
///         return dieSize == 6 ? 6 : dieSize;
///     }
/// }
/// 
/// var evaluator = DiceExpressionEvaluator&lt;int&gt;.CreateCustom(new LoadedDiceRoller());
/// </code>
/// </example>
public interface IRoller
{
    /// <summary>
    /// Rolls a die with the specified number of sides.
    /// </summary>
    /// <param name="dieSize">The number of sides on the die (e.g., 6 for a d6, 20 for a d20).</param>
    /// <returns>A value between 1 and <paramref name="dieSize"/> inclusive.</returns>
    int RollDice(int dieSize);
}