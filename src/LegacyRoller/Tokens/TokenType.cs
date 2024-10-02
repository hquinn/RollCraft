namespace LegacyRoller.Tokens;

public enum TokenType : byte
{
    Dice,
    Minimum,
    
    Minus,
    Plus,
    Asterisk,
    Slash,
    Number,
    LeftParenthesis,
    RightParenthesis,
}