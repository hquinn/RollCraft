namespace RollCraft.Rollers;

public class FixedAverageRoller : IRoller
{
    public int RollDice(int dieSize)
    {
        return dieSize / 2 + 1;
    }
}