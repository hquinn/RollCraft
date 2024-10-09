namespace LegacyRoller.Tokens;

public enum TokenType : byte
{
    Dice,
    Minimum,
    Maximum,
    Exploding,
    Equal,
    
    Minus,
    Plus,
    Asterisk,
    Slash,
    Number,
    LeftParenthesis,
    RightParenthesis,
}