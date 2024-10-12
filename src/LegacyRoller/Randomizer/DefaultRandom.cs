namespace LegacyRoller.Randomizer;

internal class DefaultRandom : IRandom
{
    public int RollDice(int dieSize)
    {
        return Random.Shared.Next(1, dieSize + 1);
    }
}