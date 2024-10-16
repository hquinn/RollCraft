namespace RollCraft.Full;

[Flags]
public enum DiceModifier
{
    None = 0,
    Minimum = 1,
    Maximum = 2,
    Exploded = 4,
    Dropped = 8,
    Rerolled = 16,
}