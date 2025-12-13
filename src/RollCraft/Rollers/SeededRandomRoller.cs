namespace RollCraft.Rollers;

/// <summary>
/// A roller that uses a seeded random number generator for reproducible results.
/// </summary>
/// <remarks>
/// This roller is thread-safe and can be used concurrently from multiple threads.
/// Using the same seed will produce the same sequence of rolls when called in the same order.
/// </remarks>
internal class SeededRandomRoller : IRoller
{
    private readonly Random _random;
    private readonly object _lock = new();
    
    internal SeededRandomRoller(int seed)
    {
        _random = new Random(seed);
    }
    
    public int RollDice(int dieSize)
    {
        lock (_lock)
        {
            return _random.Next(1, dieSize + 1);
        }
    }
}