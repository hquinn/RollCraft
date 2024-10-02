namespace LegacyRoller.Tokens;

public enum TokenType : byte
{
    Dice,
    Minimum,
    Maximum,
    
    Minus,
    Plus,
    Asterisk,
    Slash,
    Number,
    LeftParenthesis,
    RightParenthesis,
}