using System.Globalization;
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
        var start = index;

        if (!char.IsDigit(input[index]))
        {
            return null;
        }

        var hasDecimalPoint = false;
        while (index < input.Length)
        {
            var current = input[index];

            if (current == '.')
            {
                if (hasDecimalPoint)
                {
                    break;
                }

                hasDecimalPoint = true;
            }
            else if (!char.IsDigit(current))
            {
                break;
            }

            index++;
        }

        var numberSpan = input.Slice(start, index - start);

        if (double.TryParse(numberSpan, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
        {
            refIndex = index;
            return new Token(number);
        }

        return null;
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