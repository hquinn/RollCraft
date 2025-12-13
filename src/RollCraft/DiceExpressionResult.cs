using System.Numerics;

namespace RollCraft;

/// <summary>
/// Contains the result of evaluating a dice expression, including the final computed value and all individual dice rolls.
/// </summary>
/// <typeparam name="TRollError">The error type used in the result (typically <see cref="IRollError"/>).</typeparam>
/// <typeparam name="TNumber">The numeric type for the result value.</typeparam>
/// <remarks>
/// This type is immutable after construction. The <see cref="Result"/> and <see cref="Rolls"/> properties can only be set during initialization.
/// </remarks>
public sealed class DiceExpressionResult<TRollError, TNumber> where TNumber : INumber<TNumber>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiceExpressionResult{RollError, TNumber}"/> class.
    /// </summary>
    /// <param name="result">The final computed value of the expression.</param>
    /// <param name="rolls">The list of individual dice rolls that contributed to the result. Defaults to an empty list.</param>
    public DiceExpressionResult(TNumber result, List<DiceRoll>? rolls = null)
    {
        Result = result;
        Rolls = rolls ?? [];
    }
    
    /// <summary>
    /// Gets the final computed value of the evaluated expression.
    /// </summary>
    public TNumber Result { get; }
    
    /// <summary>
    /// Gets the list of individual dice rolls that were made during evaluation.
    /// </summary>
    /// <remarks>
    /// Each <see cref="DiceRoll"/> contains information about the die size, the rolled value,
    /// and any modifiers that were applied (such as exploded, dropped, or rerolled).
    /// </remarks>
    public List<DiceRoll> Rolls { get; }

    /// <summary>
    /// Returns a string representation of the result in the format "Result: [Roll1, Roll2, ...]".
    /// </summary>
    public override string ToString()
    {
        return $"{Result}: [{string.Join(", ", Rolls)}]";
    }
}