namespace LegacyRoller.UnitTests.Helpers;

public class SequentialRandom : IRandom
{
    private int _number = 1;

    public int RollDice(int dieSize)
    {
        return _number++ % dieSize;
    }
}