namespace RollCraft.Rollers;

internal class SeededRandomRoller : IRoller
{
    private readonly Random _random;
    
    internal SeededRandomRoller(int seed)
    {
        _random = new Random(seed);
    }
    
    public int RollDice(int dieSize)
    {
        return _random.Next(1, dieSize + 1);
    }
}