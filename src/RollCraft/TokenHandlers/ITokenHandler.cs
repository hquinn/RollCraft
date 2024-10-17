using System.Numerics;
using LitePrimitives;
using RollCraft.Tokens;

namespace RollCraft.TokenHandlers;

internal interface ITokenHandler
{
    Result<DiceExpression<TNumber>> ParsePrefix<TNumber>(Token<TNumber> token, ref TokenReader<TNumber> reader) 
        where TNumber : INumber<TNumber>;

    Result<DiceExpression<TNumber>> ParseInfix<TNumber>(
        DiceExpression<TNumber> left, 
        DiceExpression<TNumber> right,
        Token<TNumber> token, 
        ref TokenReader<TNumber> reader) where TNumber : INumber<TNumber>;
}