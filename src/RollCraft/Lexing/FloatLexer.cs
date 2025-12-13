using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Lexer for parsing single-precision floating-point numbers from dice expressions.
/// </summary>
internal readonly struct FloatLexer : INumberLexer<float>
{
    public static Token<float>? GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;

        var integerPart = 0L;
        var fractionalPart = 0.0f;
        var fractionalDivider = 1.0f;
        var hasDecimalPoint = false;
        var hasDigitsAfterDecimal = false;
        var hasDigits = false;

        while (index < input.Length)
        {
            var current = input[index];

            if (char.IsDigit(current))
            {
                hasDigits = true;
                var digit = current - '0';
                if (!hasDecimalPoint)
                {
                    integerPart = integerPart * 10 + digit;
                }
                else
                {
                    hasDigitsAfterDecimal = true;
                    fractionalDivider *= 10;
                    fractionalPart += digit / fractionalDivider;
                }
                index++;
            }
            else if (current == '.')
            {
                if (hasDecimalPoint)
                {
                    break; // Second decimal point encountered; invalid number
                }
                hasDecimalPoint = true;
                index++;
            }
            else
            {
                break; // Non-digit character encountered; stop parsing
            }
        }

        if (!hasDigits || (hasDecimalPoint && !hasDigitsAfterDecimal))
        {
            return null; // No digits were parsed
        }

        var number = integerPart + fractionalPart;

        refIndex = index;
        return new Token<float>(number);
    }
}
