namespace LegacyRoller.Tokens;

internal ref struct TokenReader
{
    private readonly ReadOnlySpan<Token> _tokens;

    internal TokenReader(ReadOnlySpan<Token> tokens)
    {
        _tokens = tokens;
        Position = 0;
    }

    public int Position { get; private set; }

    public bool TryPeek(out Token token, int offset = 0)
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

    public bool TryConsume(out Token token)
    {
        if (Position < _tokens.Length)
        {
            token = _tokens[Position++];
            return true;
        }

        token = default;
        return false;
    }
}