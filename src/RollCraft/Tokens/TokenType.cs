namespace RollCraft.Tokens;

public enum TokenType : byte
{
    Dice,
    Minimum,
    Maximum,
    Exploding,
    Keep,
    KeepHighest,
    KeepLowest,
    ReRoll,
    ReRollOnce,
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