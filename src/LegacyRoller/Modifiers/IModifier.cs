using LitePrimitives;

namespace LegacyRoller.Modifiers;

internal interface IModifier
{
    Result<List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls);
}