using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Lexer for parsing 16-bit integers from dice expressions.
/// </summary>
internal readonly struct ShortLexer : INumberLexer<short>
{
    public static Token<short>? GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;

        var number = 0;
        var hasDigits = false;

        while (index < input.Length)
        {
            var current = input[index];

            if (char.IsDigit(current))
            {
                hasDigits = true;
                var digit = current - '0';
                number = number * 10 + digit;
                index++;
            }
            else
            {
                break; // Non-digit character encountered; stop parsing
            }
        }

        if (!hasDigits)
        {
            return null; // No digits were parsed
        }

        refIndex = index;
        return new Token<short>((short)number);
    }
}
