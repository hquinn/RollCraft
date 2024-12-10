using System.Numerics;
using LitePrimitives;
using RollCraft.Helpers;
using RollCraft.Tokens;

namespace RollCraft.Lexing;

internal static class DiceExpressionLexer
{
    public static Result<List<Token<TNumber>>> Tokenize<TNumber, TNumberLexer>(ReadOnlySpan<char> input)
        where TNumberLexer : INumberLexer<TNumber>
        where TNumber : INumber<TNumber>
    {
        var tokens = new List<Token<TNumber>>(input.Length);
        var index = 0;

        while (index < input.Length)
        {
            if (char.IsWhiteSpace(input[index]))
            {
                index++;
                continue;
            }
            
            var token = TNumberLexer.GetNumber(input, ref index) ?? GetOperator<TNumber>(input, ref index);

            if (token is null)
            {
                return ErrorHelpers.Create("Parsing.InvalidToken", "Invalid token found", index);
            }

            tokens.Add(token.Value);
        }

        return Result<List<Token<TNumber>>>.Success(tokens);
    }
    
    private static Token<TNumber>? GetOperator<TNumber>(ReadOnlySpan<char> input, ref int refIndex)
        where TNumber : INumber<TNumber>
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
                    return new Token<TNumber>(TokenType.Minimum, TokenCategory.Modifier, 45, 45);
                }

                if (identifier.StartsWith("max", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 3;
                    return new Token<TNumber>(TokenType.Maximum, TokenCategory.Modifier, 45, 45);
                }

                return null;
            }

            case 'k' or 'K':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token<TNumber>(TokenType.Keep, TokenCategory.Modifier, 45, 45);
                }
                
                var identifier = input.Slice(index, 2);
                
                if (identifier.StartsWith("kh", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.KeepHighest, TokenCategory.Modifier, 45, 45);
                }
                
                if (identifier.StartsWith("kl", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.KeepLowest, TokenCategory.Modifier, 45, 45);
                }

                refIndex++;
                return new Token<TNumber>(TokenType.Keep, TokenCategory.Modifier, 45, 45);
            }

            case 'r' or 'R':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token<TNumber>(TokenType.ReRoll, TokenCategory.Modifier, 45, 45);
                }
                
                var identifier = input.Slice(index, 2);
                
                if (identifier.StartsWith("ro", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.ReRollOnce, TokenCategory.Modifier, 45, 45);
                }

                refIndex++;
                return new Token<TNumber>(TokenType.ReRoll, TokenCategory.Modifier, 45, 45);
            }
                
            case '!':
                refIndex++;
                return new Token<TNumber>(TokenType.Exploding, TokenCategory.Modifier, 45, 45);
            
            case '=':
                refIndex++;
                return new Token<TNumber>(TokenType.Equal, TokenCategory.Comparison, 45, 45);

            case '<':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token<TNumber>(TokenType.LesserThan, TokenCategory.Comparison, 45, 45);
                }
                
                var symbol = input.Slice(index, 2);

                if (symbol.StartsWith("<>", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.NotEqual, TokenCategory.Comparison, 45, 45);
                }

                if (symbol.StartsWith("<=", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.LesserThanEqual, TokenCategory.Comparison, 45, 45);
                }

                refIndex++;
                return new Token<TNumber>(TokenType.LesserThan, TokenCategory.Comparison, 45, 45);
            }

            case '>':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token<TNumber>(TokenType.GreaterThan, TokenCategory.Comparison, 45, 45);
                }
                
                var symbol = input.Slice(index, 2);

                if (symbol.StartsWith(">=", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.GreaterThanEqual, TokenCategory.Comparison, 45, 45);
                }

                refIndex++;
                return new Token<TNumber>(TokenType.GreaterThan, TokenCategory.Comparison, 45, 45);
            }
            
            case 'd' or 'D':
                refIndex++;
                return new Token<TNumber>(TokenType.Dice, TokenCategory.Operator, 50, 50);

            case '+':
                refIndex++;
                return new Token<TNumber>(TokenType.Plus, TokenCategory.Operator, 0, 1);

            case '-':
                refIndex++;
                return new Token<TNumber>(TokenType.Minus, TokenCategory.Operator, 80, 1);

            case '*':
                refIndex++;
                return new Token<TNumber>(TokenType.Asterisk, TokenCategory.Operator, 0, 2);

            case '/':
                refIndex++;
                return new Token<TNumber>(TokenType.Slash, TokenCategory.Operator, 0, 2);
            
            case '(':
                refIndex++;
                return new Token<TNumber>(TokenType.LeftParenthesis, TokenCategory.Parenthesis, 100, 0);

            case ')':
                refIndex++;
                return new Token<TNumber>(TokenType.RightParenthesis, TokenCategory.Parenthesis, 0, 0);
        }

        return null;
    }
}