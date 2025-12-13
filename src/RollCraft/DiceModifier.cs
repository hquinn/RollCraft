namespace RollCraft;

/// <summary>
/// Flags indicating modifications applied to a dice roll.
/// </summary>
/// <remarks>
/// Multiple modifiers can be combined using bitwise OR operations. For example, a die that was
/// rerolled and then dropped would have <c>Rerolled | Dropped</c>.
/// </remarks>
[Flags]
public enum DiceModifier
{
    /// <summary>
    /// No modifier was applied to the roll.
    /// </summary>
    None = 0,
    
    /// <summary>
    /// The minimum modifier was applied, raising the roll to a minimum value.
    /// </summary>
    Minimum = 1,
    
    /// <summary>
    /// The maximum modifier was applied, capping the roll at a maximum value.
    /// </summary>
    Maximum = 2,
    
    /// <summary>
    /// The die exploded (rolled maximum and triggered an additional roll).
    /// </summary>
    Exploded = 4,
    
    /// <summary>
    /// The die was dropped and not counted in the final total (e.g., from keep highest/lowest).
    /// </summary>
    Dropped = 8,
    
    /// <summary>
    /// The die was rerolled due to a reroll modifier condition.
    /// </summary>
    Rerolled = 16,
}