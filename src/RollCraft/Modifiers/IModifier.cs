using LitePrimitives;

namespace RollCraft.Modifiers;

internal interface IModifier
{
    Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls);
}