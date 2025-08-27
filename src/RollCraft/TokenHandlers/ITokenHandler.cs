using System.Numerics;
using MonadCraft;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal interface ITokenHandler
{
    Result<IRollError, DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader) 
        where TNumber : INumber<TNumber>;

    Result<IRollError, DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right,
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>;
}