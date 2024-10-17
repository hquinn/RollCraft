using System.Numerics;
using RollCraft.Tokens;

namespace RollCraft.Lexing;

internal interface INumberLexer<TNumber>
    where TNumber : INumber<TNumber>
{
    static abstract Token<TNumber>? GetNumber(ReadOnlySpan<char> input, ref int refIndex);
}