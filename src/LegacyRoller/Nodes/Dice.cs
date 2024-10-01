namespace LegacyRoller.Nodes;

public class Dice : DiceExpression
{
    internal DiceExpression CountOfDice { get; }
    internal DiceExpression CountOfSides { get; }

    internal Dice(DiceExpression countOfDice, DiceExpression countOfSides)
    {
        CountOfDice = countOfDice;
        CountOfSides = countOfSides;
    }
    
    public override string ToString()
    {
        return $"DICE({CountOfDice}, {CountOfSides})";
    }
}