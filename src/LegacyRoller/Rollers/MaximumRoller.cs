namespace LegacyRoller.Rollers;

public class MaximumRoller : IRoller
{
    public int RollDice(int dieSize)
    {
        return dieSize;
    }
}