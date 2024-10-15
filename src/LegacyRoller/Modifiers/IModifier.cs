using LitePrimitives;

namespace LegacyRoller.Modifiers;

internal interface IModifier
{
    Result<Unit> Modify(IRandom random, List<DiceRoll> diceRolls);
}