namespace RollCraft.Helpers;

internal static class DiceHelpers
{
    public static bool HasDiceDropped(this DiceRoll diceRoll) => (diceRoll.Modifier & DiceModifier.Dropped) != 0;
    public static bool HasDiceExploded(this DiceRoll diceRoll) => (diceRoll.Modifier & DiceModifier.Exploded) != 0;
}