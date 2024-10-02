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
    
    private static Token? GetOperator(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;

        if (index >= input.Length)
        {
            return null;
        }

        var currentChar = input[index];

        switch (currentChar)
        {
            case 'm' or 'M':
                var identifier = input.Slice(index, 3);
                
                if (identifier.StartsWith("min", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 3;
                    return new Token(TokenType.Minimum, 45, 45);
                }

                return null;
                
            case 'd' or 'D':
                refIndex++;
                return new Token(TokenType.Dice, 50, 50);

            case '+':
                refIndex++;
                return new Token(TokenType.Plus, 0, 1);

            case '-':
                refIndex++;
                return new Token(TokenType.Minus, 80, 1);

            case '*':
                refIndex++;
                return new Token(TokenType.Asterisk, 0, 2);

            case '/':
                refIndex++;
                return new Token(TokenType.Slash, 0, 2);
            
            case '(':
                refIndex++;
                return new Token(TokenType.LeftParenthesis, 100, 0);

            case ')':
                refIndex++;
                return new Token(TokenType.RightParenthesis, 0, 0);
        }

        return null;
    }
}