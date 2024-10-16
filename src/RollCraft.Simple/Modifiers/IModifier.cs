using LitePrimitives;

namespace RollCraft.Simple.Modifiers;

internal interface IModifier
{
    Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls);
}