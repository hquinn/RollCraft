using RollCraft.Tokens;

namespace RollCraft.Lexing;

internal readonly struct IntLexer : INumberLexer<int>
{
    public static Token<int>? GetNumber(ReadOnlySpan<char> input, ref int refIndex)
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
        return new Token<int>(number);
    }
}