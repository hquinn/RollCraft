using System.Numerics;
using MonadCraft;
using RollCraft.Tokens;

namespace RollCraft.Lexing;

internal static class DiceExpressionLexer
{
    public static Result<IRollError, List<Token<TNumber>>> Tokenize<TNumber, TNumberLexer>(ReadOnlySpan<char> input)
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
            
            var token = TNumberLexer.GetNumber(input, ref index) 
                        ?? GetVariable<TNumber>(input, ref index) 
                        ?? GetOperator<TNumber>(input, ref index);

            if (token is null)
            {
                return new ParserError("Parsing.InvalidToken", "Invalid token found", index);
            }

            tokens.Add(token.Value);
        }

        return Result<IRollError, List<Token<TNumber>>>.Success(tokens);
    }
    
    private static Token<TNumber>? GetVariable<TNumber>(ReadOnlySpan<char> input, ref int refIndex)
        where TNumber : INumber<TNumber>
    {
        var index = refIndex;

        if (index >= input.Length || input[index] != '[')
        {
            return null;
        }

        index++; // Skip opening bracket
        var startIndex = index;

        while (index < input.Length && input[index] != ']')
        {
            index++;
        }

        if (index >= input.Length)
        {
            return null; // No closing bracket found
        }

        var variableName = input.Slice(startIndex, index - startIndex).ToString();
        
        if (string.IsNullOrWhiteSpace(variableName))
        {
            return null; // Empty variable name
        }

        refIndex = index + 1; // Skip closing bracket
        return new Token<TNumber>(variableName);
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
            case 'a' or 'A':
            {
                if (index + 2 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 3);

                if (identifier.StartsWith("abs", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 3;
                    return new Token<TNumber>(TokenType.Abs, TokenCategory.Operator, 100, 0);
                }

                break;
            }
            
            case 'c' or 'C':
            {
                if (index + 3 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 4);

                if (identifier.StartsWith("ceil", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 4;
                    return new Token<TNumber>(TokenType.Ceil, TokenCategory.Operator, 100, 0);
                }

                break;
            }
            
            case 'f' or 'F':
            {
                if (index + 4 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 5);

                if (identifier.StartsWith("floor", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 5;
                    return new Token<TNumber>(TokenType.Floor, TokenCategory.Operator, 100, 0);
                }

                break;
            }
            
            case 'i' or 'I':
            {
                if (index + 1 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 2);

                if (identifier.StartsWith("if", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.If, TokenCategory.Operator, 100, 0);
                }

                break;
            }
            
            case 'm' or 'M':
            {
                if (index + 2 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 3);

                if (identifier.StartsWith("min", StringComparison.OrdinalIgnoreCase))
                {
                    // Only treat as function if followed by '(' AND (at start OR preceded by whitespace/operator)
                    // Otherwise, treat as a modifier (e.g., 1d6min3)
                    var isFunction = index + 3 < input.Length && input[index + 3] == '(' &&
                                     (index == 0 || !char.IsLetterOrDigit(input[index - 1]));
                    
                    if (isFunction)
                    {
                        refIndex += 3;
                        return new Token<TNumber>(TokenType.FuncMin, TokenCategory.Operator, 100, 0);
                    }
                    
                    refIndex += 3;
                    return new Token<TNumber>(TokenType.Minimum, TokenCategory.Modifier, 45, 45);
                }

                if (identifier.StartsWith("max", StringComparison.OrdinalIgnoreCase))
                {
                    // Only treat as function if followed by '(' AND (at start OR preceded by whitespace/operator)
                    // Otherwise, treat as a modifier (e.g., 1d6max3)
                    var isFunction = index + 3 < input.Length && input[index + 3] == '(' &&
                                     (index == 0 || !char.IsLetterOrDigit(input[index - 1]));
                    
                    if (isFunction)
                    {
                        refIndex += 3;
                        return new Token<TNumber>(TokenType.FuncMax, TokenCategory.Operator, 100, 0);
                    }
                    
                    refIndex += 3;
                    return new Token<TNumber>(TokenType.Maximum, TokenCategory.Modifier, 45, 45);
                }


                break;
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
                // Check for round function first (longer match)
                if (index + 4 < input.Length)
                {
                    var longerIdentifier = input.Slice(index, 5);
                    if (longerIdentifier.StartsWith("round", StringComparison.OrdinalIgnoreCase))
                    {
                        refIndex += 5;
                        return new Token<TNumber>(TokenType.Round, TokenCategory.Operator, 100, 0);
                    }
                }
                
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
            
            case 's' or 'S':
            {
                if (index + 3 >= input.Length)
                {
                    return null;
                }
                
                var identifier = input.Slice(index, 4);

                if (identifier.StartsWith("sqrt", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 4;
                    return new Token<TNumber>(TokenType.Sqrt, TokenCategory.Operator, 100, 0);
                }

                break;
            }
                
            case '!':
                refIndex++;
                return new Token<TNumber>(TokenType.Exploding, TokenCategory.Modifier, 45, 45);
            
            case '=':
                refIndex++;
                return new Token<TNumber>(TokenType.Equal, TokenCategory.Comparison, 0, 0);

            case '<':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token<TNumber>(TokenType.LesserThan, TokenCategory.Comparison, 0, 0);
                }
                
                var symbol = input.Slice(index, 2);

                if (symbol.StartsWith("<>", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.NotEqual, TokenCategory.Comparison, 0, 0);
                }

                if (symbol.StartsWith("<=", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.LesserThanEqual, TokenCategory.Comparison, 0, 0);
                }

                refIndex++;
                return new Token<TNumber>(TokenType.LesserThan, TokenCategory.Comparison, 0, 0);
            }

            case '>':
            {
                if (index + 1 >= input.Length)
                {
                    refIndex++;
                    return new Token<TNumber>(TokenType.GreaterThan, TokenCategory.Comparison, 0, 0);
                }
                
                var symbol = input.Slice(index, 2);

                if (symbol.StartsWith(">=", StringComparison.OrdinalIgnoreCase))
                {
                    refIndex += 2;
                    return new Token<TNumber>(TokenType.GreaterThanEqual, TokenCategory.Comparison, 0, 0);
                }

                refIndex++;
                return new Token<TNumber>(TokenType.GreaterThan, TokenCategory.Comparison, 0, 0);
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
            
            case '%':
                refIndex++;
                return new Token<TNumber>(TokenType.Percent, TokenCategory.Operator, 0, 2);
            
            case '(':
                refIndex++;
                return new Token<TNumber>(TokenType.LeftParenthesis, TokenCategory.Parenthesis, 100, 0);

            case ')':
                refIndex++;
                return new Token<TNumber>(TokenType.RightParenthesis, TokenCategory.Parenthesis, 0, 0);
            
            case ',':
                refIndex++;
                return new Token<TNumber>(TokenType.Comma, TokenCategory.Operator, 0, 0);
        }

        return null;
    }
}