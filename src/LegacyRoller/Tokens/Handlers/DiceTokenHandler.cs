using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller.Tokens.Handlers;

internal sealed class DiceTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        // Treat 'dX' as '1dX'
        var oneExpression = new Number(1);

        var rightResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.PrefixPrecedence);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        var diceExpression = new Dice(oneExpression, rightResult.Value!);

        return Result<DiceExpression>.Success(diceExpression);
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Dice(left, right));
    }
}