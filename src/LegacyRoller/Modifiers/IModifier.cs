using LitePrimitives;

namespace LegacyRoller.Modifiers;

internal interface IModifier
{
    Result<Unit> Modify(IRoller roller, List<DiceRoll> diceRolls);
}