using LitePrimitives;
using RollCraft.Simple.Tokens;

namespace RollCraft.Simple.TokenHandlers;

internal interface ITokenHandler
{
    Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader);
    Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader);
}