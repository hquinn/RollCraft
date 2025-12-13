using RollCraft.Tokens;

namespace RollCraft.Lexing;

/// <summary>
/// Lexer for parsing 64-bit integers from dice expressions.
/// </summary>
internal readonly struct LongLexer : INumberLexer<long>
{
    private const long MaxValueBeforeMultiply = long.MaxValue / 10;
    private const long MaxLastDigit = long.MaxValue % 10;
    
    public static NumberLexResult<long> GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;
        var startIndex = index;

        var number = 0L;
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
                    return NumberLexResult<long>.Overflow(startIndex);
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
            return NumberLexResult<long>.NoMatch;
        }

        refIndex = index;
        return NumberLexResult<long>.Success(new Token<long>(number));
    }
}
