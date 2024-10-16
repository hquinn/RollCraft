using LitePrimitives;
using RollCraft.Full.Nodes;
using RollCraft.Full.Tokens;

namespace RollCraft.Full.TokenHandlers;

internal sealed class MinusTokenHandler : ITokenHandler
{
    public Result<DiceExpression> ParsePrefix(Token token, ref TokenReader reader)
    {
        if (!reader.TryPeek(out var nextToken))
        {
            return Result<DiceExpression>.Failure(
                new ParserError("UnexpectedEnd", "Unexpected end of input", reader.Position));
        }

        // We want to treat '-dX' as '-1dX'
        if (nextToken.TokenDetails.TokenType == TokenType.Dice)
        {
            return Result<DiceExpression>.Success(new Unary(new Number(1)));
        }
        
        var operandResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.PrefixPrecedence);
        return operandResult.IsFailure 
            ? operandResult 
            : Result<DiceExpression>.Success(new Unary(operandResult.Value!));
    }

    public Result<DiceExpression> ParseInfix(DiceExpression left, DiceExpression right, Token token, ref TokenReader reader)
    {
        return Result<DiceExpression>.Success(new Subtract(left, right));
    }
}