using System.Text;

namespace RollCraft;

/// <summary>
/// Represents a single dice roll, including the die size, rolled value, and any modifiers applied.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="ToString"/> method produces a string representation with modifier indicators:
/// </para>
/// <list type="bullet">
/// <item><description><c>^</c> - Minimum modifier was applied</description></item>
/// <item><description><c>v</c> - Maximum modifier was applied</description></item>
/// <item><description><c>!</c> - Die exploded (rolled maximum and was rerolled)</description></item>
/// <item><description><c>d</c> - Die was dropped (not counted in total)</description></item>
/// <item><description><c>r</c> - Die was rerolled</description></item>
/// </list>
/// </remarks>
public sealed class DiceRoll : IEquatable<DiceRoll>, IComparable<DiceRoll>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiceRoll"/> class.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <param name="roll">The value that was rolled.</param>
    /// <param name="modifier">Any modifiers applied to this roll. Defaults to <see cref="DiceModifier.None"/>.</param>
    public DiceRoll(int sides, int roll, DiceModifier modifier = DiceModifier.None)
    {
        Sides = sides;
        Roll = roll;
        Modifier = modifier;
    }

    /// <summary>
    /// Gets the number of sides on the die (e.g., 6 for a d6, 20 for a d20).
    /// </summary>
    public int Sides { get; internal set; }
    
    /// <summary>
    /// Gets the value that was rolled.
    /// </summary>
    public int Roll { get; internal set; }
    
    /// <summary>
    /// Gets the modifiers that were applied to this roll.
    /// </summary>
    public DiceModifier Modifier { get; internal set; }
    
    /// <summary>
    /// Returns a string representation of the roll with modifier indicators.
    /// </summary>
    public override string ToString()
    {
        var result = new StringBuilder(Roll.ToString());
        
        if (Modifier != DiceModifier.None)
        {
            if ((Modifier & DiceModifier.Minimum) != 0) result.Append('^');
            if ((Modifier & DiceModifier.Maximum) != 0) result.Append('v');
            if ((Modifier & DiceModifier.Exploded) != 0) result.Append('!');
            if ((Modifier & DiceModifier.Dropped) != 0) result.Append('d');
            if ((Modifier & DiceModifier.Rerolled) != 0) result.Append('r');
        }

        return result.ToString();
    }
    
    /// <summary>
    /// Compares this roll to another based on the rolled value.
    /// </summary>
    public int CompareTo(DiceRoll? other)
    {
        return Roll.CompareTo(other!.Roll);
    }

    /// <summary>
    /// Determines whether this roll equals another roll.
    /// </summary>
    public bool Equals(DiceRoll? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Sides == other.Sides && Roll == other.Roll && Modifier == other.Modifier;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DiceRoll other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Sides, Roll, (int)Modifier);
    }
}