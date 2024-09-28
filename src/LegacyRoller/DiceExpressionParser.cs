using LegacyRoller.Errors;
using LegacyRoller.Nodes;
using LitePrimitives;

namespace LegacyRoller;

public static class DiceExpressionParser
{
    public static Result<DiceExpression> Parse(string input)
    {
        var tokensResult = DiceExpressionLexer.Tokenize(input.AsSpan());

        return tokensResult.Bind(ParseExpression);
    }
    
    private static Result<DiceExpression> ParseExpression(List<Token> tokens)
    {
        var tokenIndex = 0;
        return ParseExpression(tokens, 0, ref tokenIndex);
    }

    private static Result<DiceExpression> ParseExpression(List<Token> tokens, int precedence, ref int tokenIndex)
    {
        var token = tokens[tokenIndex];
        tokenIndex = AdvanceTokenIndex(tokenIndex, tokens.Count);
        
        var leftResult = ParsePrefix(token, tokens, ref tokenIndex);

        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        while (precedence < GetPrecedence(tokens[tokenIndex]))
        {
            token = tokens[tokenIndex];
            tokenIndex = AdvanceTokenIndex(tokenIndex, tokens.Count);
            leftResult = ParseInfix(leftResult.Value!, token, tokens, ref tokenIndex);
            
            if (leftResult.IsFailure)
            {
                return leftResult;
            }
        }
        
        return leftResult;
    }
    
    private static Result<DiceExpression> ParsePrefix(Token token, List<Token> tokens, ref int tokenIndex)
    {
        switch (token.TokenType)
        {
            case TokenType.Number:
                return Result<DiceExpression>.Success(new Number(token.Value));
            case TokenType.Minus:
                var precedence = GetPrefixPrecedence(token);
                return ParseExpression(tokens, precedence, ref tokenIndex)
                    .Match(
                        success: right => Result<DiceExpression>.Success(new Unary(right)),
                        failure: Result<DiceExpression>.Failure);
            default:
                return Result<DiceExpression>.Failure(
                    new ParserError("InvalidToken", $"Invalid token found {token.TokenType}", tokenIndex));
        }
    }
    
    private static Result<DiceExpression> ParseInfix(DiceExpression left, Token token, List<Token> tokens, ref int tokenIndex)
    {
        var precedence = GetInfixPrecedence(token);
        var rightResult = ParseExpression(tokens, precedence, ref tokenIndex);
        var index = tokenIndex;
        
        return rightResult.Bind<DiceExpression>(
            bindFunc: right =>
            {
                DiceExpression expression;

                switch (token.TokenType)
                {
                    case TokenType.Minus:
                        expression = new Subtract(left, right);
                        break;
                    case TokenType.Add:
                        expression = new Add(left, right);
                        break;
                    default:
                        return Result<DiceExpression>.Failure(new ParserError("InvalidToken", $"Invalid token found {token.TokenType}", index));
                }
                
                return Result<DiceExpression>.Success(expression);
            });
    }
    
    private static int GetPrecedence(Token token)
    {
        var infix = GetInfixPrecedence(token);
        var prefix = GetPrefixPrecedence(token);
        
        return Math.Max(infix, prefix);
    }
    
    private static int GetInfixPrecedence(Token token)
    {
        return token.TokenType switch
        {
            TokenType.Minus => 1,
            TokenType.Add => 1,
            _ => 0
        };
    }
    
    private static int GetPrefixPrecedence(Token token)
    {
        return token.TokenType switch
        {
            // Make sure prefix precedence is higher than infix precedence
            TokenType.Minus => 2,
            _ => 0
        };
    }
    
    private static int AdvanceTokenIndex(int tokenIndex, int count)
    {
        if (tokenIndex < count - 1)
        {
            return tokenIndex + 1;
        }
        
        return tokenIndex;
    }
}