using System.Numerics;
using LitePrimitives;
using RollCraft.Nodes;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal sealed class MinusTokenHandler : ITokenHandler
{
    public Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader)
        where TNumber : INumber<TNumber>
    {
        if (!reader.TryPeek(out var nextToken))
        {
            return Result<DiceExpression<TNumber>>.Failure(
                new ParserError("UnexpectedEnd", "Unexpected end of input", reader.Position));
        }

        // We want to treat '-dX' as '-1dX'
        if (nextToken.TokenDetails.TokenType == TokenType.Dice)
        {
            return Result<DiceExpression<TNumber>>.Success(new Unary<TNumber>(new Number<TNumber>(TNumber.One)));
        }
        
        var operandResult = DiceExpressionParser.ParseExpression(ref reader, token.TokenDetails.PrefixPrecedence);
        return operandResult.IsFailure 
            ? operandResult 
            : Result<DiceExpression<TNumber>>.Success(new Unary<TNumber>(operandResult.Value!));
    }

    public Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right, 
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>
    {
        return Result<DiceExpression<TNumber>>.Success(new Subtract<TNumber>(left, right));
    }
}