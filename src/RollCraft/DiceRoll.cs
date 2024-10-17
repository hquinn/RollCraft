using System.Text;

namespace RollCraft;

public sealed class DiceRoll : IEquatable<DiceRoll>, IComparable<DiceRoll>
{
    public DiceRoll(int sides, int roll, DiceModifier modifier = DiceModifier.None)
    {
        Sides = sides;
        Roll = roll;
        Modifier = modifier;
    }

    public int Sides { get; internal set; }
    public int Roll { get; internal set; }
    public DiceModifier Modifier { get; internal set; }
    
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
    
    public int CompareTo(DiceRoll? other)
    {
        return Roll.CompareTo(other!.Roll);
    }

    public bool Equals(DiceRoll? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Sides == other.Sides && Roll == other.Roll && Modifier == other.Modifier;
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is DiceRoll other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Sides, Roll, (int)Modifier);
    }
}