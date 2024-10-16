using LitePrimitives;

namespace RollCraft.Full.Modifiers;

internal interface IModifier
{
    Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls);
}