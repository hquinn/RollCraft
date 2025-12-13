using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Lexer for parsing 16-bit integers from dice expressions.
/// </summary>
internal readonly struct ShortLexer : INumberLexer<short>
{
    private const int MaxValueBeforeMultiply = short.MaxValue / 10;
    private const int MaxLastDigit = short.MaxValue % 10;
    
    public static NumberLexResult<short> GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;
        var startIndex = index;

        var number = 0;
        var hasDigits = false;

        while (index < input.Length)
        {
            var current = input[index];

            if (char.IsDigit(current))
            {
                hasDigits = true;
                var digit = current - '0';
                
                // Check for overflow before the operation
                // Overflow occurs if: number > MaxValue/10 OR (number == MaxValue/10 AND digit > MaxValue%10)
                if (number > MaxValueBeforeMultiply || (number == MaxValueBeforeMultiply && digit > MaxLastDigit))
                {
                    return NumberLexResult<short>.Overflow(startIndex);
                }
                
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
            return NumberLexResult<short>.NoMatch;
        }

        refIndex = index;
        return NumberLexResult<short>.Success(new Token<short>((short)number));
    }
}
