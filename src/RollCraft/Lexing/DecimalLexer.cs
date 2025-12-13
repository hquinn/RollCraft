using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Lexer for parsing decimal numbers from dice expressions.
/// </summary>
internal readonly struct DecimalLexer : INumberLexer<decimal>
{
    // Decimal max is 79228162514264337593543950335
    // We use a simpler approach: track the number of digits
    private const int MaxDecimalDigits = 29;
    
    public static NumberLexResult<decimal> GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;
        var startIndex = index;

        var integerPart = 0m;
        var fractionalPart = 0m;
        var fractionalDivider = 1m;
        var hasDecimalPoint = false;
        var hasDigitsAfterDecimal = false;
        var hasDigits = false;
        var digitCount = 0;

        while (index < input.Length)
        {
            var current = input[index];

            if (char.IsDigit(current))
            {
                hasDigits = true;
                var digit = current - '0';
                
                if (!hasDecimalPoint)
                {
                    // Skip leading zeros for digit count
                    if (integerPart != 0 || digit != 0)
                    {
                        digitCount++;
                    }
                    
                    // Check for potential overflow
                    if (digitCount > MaxDecimalDigits)
                    {
                        return NumberLexResult<decimal>.Overflow(startIndex);
                    }
                    
                    try
                    {
                        integerPart = integerPart * 10 + digit;
                    }
                    catch (OverflowException)
                    {
                        return NumberLexResult<decimal>.Overflow(startIndex);
                    }
                }
                else
                {
                    hasDigitsAfterDecimal = true;
                    try
                    {
                        fractionalDivider *= 10;
                        fractionalPart += digit / fractionalDivider;
                    }
                    catch (OverflowException)
                    {
                        // Fractional overflow is less common, but handle it
                        return NumberLexResult<decimal>.Overflow(startIndex);
                    }
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
            return NumberLexResult<decimal>.NoMatch;
        }

        var number = integerPart + fractionalPart;

        refIndex = index;
        return NumberLexResult<decimal>.Success(new Token<decimal>(number));
    }
}
