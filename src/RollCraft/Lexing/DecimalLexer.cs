using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Lexer for parsing decimal numbers from dice expressions.
/// </summary>
internal readonly struct DecimalLexer : INumberLexer<decimal>
{
    public static Token<decimal>? GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;

        var integerPart = 0L;
        var fractionalPart = 0m;
        var fractionalDivider = 1m;
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
        return new Token<decimal>(number);
    }
}
