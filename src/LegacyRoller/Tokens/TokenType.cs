namespace LegacyRoller.Tokens;

public enum TokenType : byte
{
    Dice,
    Minimum,
    Maximum,
    Exploding,
    Keep,
    KeepHighest,
    KeepLowest,
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanEqual,
    LesserThan,
    LesserThanEqual,
    
    Minus,
    Plus,
    Asterisk,
    Slash,
    Number,
    LeftParenthesis,
    RightParenthesis,
}