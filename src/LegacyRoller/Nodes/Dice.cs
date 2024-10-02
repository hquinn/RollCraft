using LegacyRoller.Modifiers;

namespace LegacyRoller.Nodes;

internal sealed class Dice : DiceExpression
{
    internal DiceExpression CountOfDice { get; }
    internal DiceExpression CountOfSides { get; }
    
    internal List<IModifier> Modifiers { get; } = new();

    internal Dice(DiceExpression countOfDice, DiceExpression countOfSides)
    {
        CountOfDice = countOfDice;
        CountOfSides = countOfSides;
    }
    
    public override string ToString()
    {
        var modifiers = string.Join(", ", Modifiers);
        return $"DICE({CountOfDice}, {CountOfSides}{(string.IsNullOrEmpty(modifiers) ? "" : $", {modifiers}")})";
    }
}