namespace RollCraft.UnitTests.Helpers;

public class ExactRoller : IRoller
{
    private readonly int[] _rolls;
    private int _index = 0;

    public ExactRoller(int[] rolls)
    {
        _rolls = rolls;
    }

    public int RollDice(int dieSize)
    {
        if (_index >= _rolls.Length)
        {
            throw new InvalidOperationException("No more rolls available");
        }
        
        return _rolls[_index++];
    }
}