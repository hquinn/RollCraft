namespace RollCraft.Full.Rollers;

internal class RandomRoller : IRoller
{
    public int RollDice(int dieSize)
    {
        return Random.Shared.Next(1, dieSize + 1);
    }
}