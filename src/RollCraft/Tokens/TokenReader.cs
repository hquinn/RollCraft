using System.Numerics;

namespace RollCraft.Tokens;

internal ref struct TokenReader<TNumber> where  TNumber : INumber<TNumber>
{
    private readonly ReadOnlySpan<Token<TNumber>> _tokens;

    internal TokenReader(ReadOnlySpan<Token<TNumber>> tokens)
    {
        _tokens = tokens;
        Position = 0;
    }

    public int Position { get; private set; }
    
    public bool TryPeek(out Token<TNumber> token, int offset = 0)
    {
        var index = Position + offset;
        if (index < _tokens.Length)
        {
            token = _tokens[index];
            return true;
        }

        token = default;
        return false;
    }

    public bool TryConsume(out Token<TNumber> token)
    {
        if (Position < _tokens.Length)
        {
            token = _tokens[Position++];
            return true;
        }

        token = default;
        return false;
    }
    
    public void Advance()
    {
        Position++;
    }
}