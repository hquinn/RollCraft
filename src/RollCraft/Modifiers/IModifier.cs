using MonadCraft;

namespace RollCraft.Modifiers;

internal interface IModifier
{
    Result<IRollError, List<DiceRoll>> Modify(IRoller roller, List<DiceRoll> diceRolls);
}