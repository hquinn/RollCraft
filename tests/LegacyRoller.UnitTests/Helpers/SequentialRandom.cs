namespace LegacyRoller.UnitTests.Helpers;

public class SequentialRandom : IRandom
{
    private int _number = 0;

    public int RollDice(int dieSize)
    {
        return (_number++ % dieSize) + 1;
    }
}