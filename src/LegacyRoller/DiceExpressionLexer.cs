using LegacyRoller.Errors;
using LegacyRoller.Tokens;
using LitePrimitives;

namespace LegacyRoller;

public static class DiceExpressionLexer
{
    public static Result<List<Token>> Tokenize(ReadOnlySpan<char> input)
    {
        var tokens = new List<Token>(input.Length);
        var index = 0;

        while (index < input.Length)
        {
            if (char.IsWhiteSpace(input[index]))
            {
                index++;
                continue;
            }
            
            var token = GetNumber(input, ref index) ?? GetOperator(input, ref index);

            if (token is null)
            {
                return Result<List<Token>>.Failure(new LexerError("InvalidToken", "Invalid token found", index));
            }
            
            tokens.Add(token.Value);
        }

        return Result<List<Token>>.Success(tokens);
    }

    private static Token? GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;

        var integerPart = 0L;
        var fractionalPart = 0.0;
        var fractionalDivider = 1.0;
        var hasDecimalPoint = false;
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

        if (!hasDigits)
        {
            return null; // No digits were parsed
        }

        var number = integerPart + fractionalPart;

        refIndex = index;
        return new Token(number);
    }

    private static readonly (string OperatorString, TokenType TokenType, byte Prefix, byte Infix)[] SortedOperators =
    [
        ("d", TokenType.Dice, 4, 4),
        ("-", TokenType.Minus, 6, 1),
        ("+", TokenType.Plus, 0, 1),
        ("*", TokenType.Asterisk, 0, 2),
        ("/", TokenType.Slash, 0, 2)
    ];

    private static Token? GetOperator(ReadOnlySpan<char> input, ref int refIndex)
    {
        foreach (var (opString, tokenType, prefix, infix) in SortedOperators)
        {
            var opLength = opString.Length;
            if (refIndex + opLength <= input.Length &&
                input.Slice(refIndex, opLength).StartsWith(opString.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
            {
                refIndex += opLength;
                return new Token(tokenType, prefix, infix);
            }
        }
        
        return null;
    }
}