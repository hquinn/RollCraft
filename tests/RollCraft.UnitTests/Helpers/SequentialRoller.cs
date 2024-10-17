namespace RollCraft.UnitTests.Helpers;

public class SequentialRoller : IRoller
{
    private int _number = 0;

    public int RollDice(int dieSize)
    {
        return (_number++ % dieSize) + 1;
    }
}