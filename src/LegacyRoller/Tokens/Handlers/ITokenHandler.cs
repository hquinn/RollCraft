using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal interface ITokenHandler
{
    Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader);
    Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader);
}