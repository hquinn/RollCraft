using LegacyRoller.Errors;
using LitePrimitives;

namespace LegacyRoller;

internal static class DiceExpressionLexer
{
    internal static Result<List<Token>> Tokenize(ReadOnlySpan<char> input)
    {
        var tokens = new List<Token>();
        var index = 0;

        while (index < input.Length)
        {
            if (char.IsWhiteSpace(input[index]))
            {
                index++;
                continue;
            }
            
            var tokenOption = GetNumber(input, ref index);

            if (tokenOption.IsNone)
            {
                tokenOption = GetOperator(input, ref index);
            }

            if (tokenOption.IsNone)
            {
                return Result<List<Token>>.Failure(new LexerError("InvalidToken", "Invalid token found", index));
            }
            
            tokenOption.OnSome(token => tokens.Add(token));
        }

        return Result<List<Token>>.Success(tokens);
    }

    private static Option<Token> GetNumber(ReadOnlySpan<char> input, ref int refIndex)
    {
        var index = refIndex;
        var start = index;

        if (!char.IsDigit(input[index]))
        {
            return Option<Token>.None();
        }
        
        do
        {
            var current = input[index];
            
            if (current != '.' && !char.IsDigit(current))
            {
                break;
            }
            
            index++;
        } while (index < input.Length);
        
        if (double.TryParse(input.Slice(start, index - start), out var number))
        {
            refIndex = index;
            return Option<Token>.Some(new Token(TokenType.Number, number));
        }
        
        return Option<Token>.None();
    }
    
    private static readonly Dictionary<string, TokenType> Operators = new()
    {
        { "-", TokenType.Minus },
        { "+", TokenType.Plus },
        { "*", TokenType.Asterisk },
        { "/", TokenType.Slash }
    };

    private static Option<Token> GetOperator(ReadOnlySpan<char> input, ref int refIndex)
    {
        // List of operator strings sorted by length in decreasing order
        var operatorStrings = Operators.Keys.OrderByDescending(op => op.Length);

        foreach (var op in operatorStrings)
        {
            var opSpan = op.AsSpan();
            
            if (refIndex + opSpan.Length <= input.Length &&
                input.Slice(refIndex, opSpan.Length).SequenceEqual(opSpan))
            {
                var tokenType = Operators[op];
                refIndex += opSpan.Length;
                return Option<Token>.Some(new Token(tokenType));
            }
        }
        
        return Option<Token>.None();
    }
}