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
            {
                if (index + 2 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 3);

                if (identifier.StartsWith("min", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 3;
                    return new Token(TokenType.Minimum, TokenCategory.Modifier, 45, 45);
                }

                if (identifier.StartsWith("max", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 3;
                    return new Token(TokenType.Maximum, TokenCategory.Modifier, 45, 45);
                }

                return null;
            }

            case 'k' or 'K':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token(TokenType.Keep, TokenCategory.Modifier, 45, 45);
                }
                
                var identifier = input.Slice(index, 2);
                
                if (identifier.StartsWith("kh", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token(TokenType.KeepHighest, TokenCategory.Modifier, 45, 45);
                }
                
                if (identifier.StartsWith("kl", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token(TokenType.KeepLowest, TokenCategory.Modifier, 45, 45);
                }

                refIndex++;
                return new Token(TokenType.Keep, TokenCategory.Modifier, 45, 45);
            }

            case 'r' or 'R':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token(TokenType.ReRoll, TokenCategory.Modifier, 45, 45);
                }
                
                var identifier = input.Slice(index, 2);
                
                if (identifier.StartsWith("ro", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token(TokenType.ReRollOnce, TokenCategory.Modifier, 45, 45);
                }

                refIndex++;
                return new Token(TokenType.ReRoll, TokenCategory.Modifier, 45, 45);
            }
                
            case '!':
                refIndex++;
                return new Token(TokenType.Exploding, TokenCategory.Modifier, 45, 45);
            
            case '=':
                refIndex++;
                return new Token(TokenType.Equal, TokenCategory.Comparison, 40, 40);

            case '<':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token(TokenType.LesserThan, TokenCategory.Comparison, 40, 40);
                }
                
                var symbol = input.Slice(index, 2);

                if (symbol.StartsWith("<>", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token(TokenType.NotEqual, TokenCategory.Comparison, 40, 40);
                }

                if (symbol.StartsWith("<=", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token(TokenType.LesserThanEqual, TokenCategory.Comparison, 40, 40);
                }

                refIndex++;
                return new Token(TokenType.LesserThan, TokenCategory.Comparison, 40, 40);
            }

            case '>':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token(TokenType.GreaterThan, TokenCategory.Comparison, 40, 40);
                }
                
                var symbol = input.Slice(index, 2);

                if (symbol.StartsWith(">=", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token(TokenType.GreaterThanEqual, TokenCategory.Comparison, 40, 40);
                }

                refIndex++;
                return new Token(TokenType.GreaterThan, TokenCategory.Comparison, 40, 40);
            }
            
            case 'd' or 'D':
                refIndex++;
                return new Token(TokenType.Dice, TokenCategory.Operator, 50, 50);

            case '+':
                refIndex++;
                return new Token(TokenType.Plus, TokenCategory.Operator, 0, 1);

            case '-':
                refIndex++;
                return new Token(TokenType.Minus, TokenCategory.Operator, 80, 1);

            case '*':
                refIndex++;
                return new Token(TokenType.Asterisk, TokenCategory.Operator, 0, 2);

            case '/':
                refIndex++;
                return new Token(TokenType.Slash, TokenCategory.Operator, 0, 2);
            
            case '(':
                refIndex++;
                return new Token(TokenType.LeftParenthesis, TokenCategory.Parenthesis, 100, 0);

            case ')':
                refIndex++;
                return new Token(TokenType.RightParenthesis, TokenCategory.Parenthesis, 0, 0);
        }

        return null;
    }
}