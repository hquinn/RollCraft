namespace LegacyRoller.Tokens;

internal enum TokenCategory : byte
{
    Operand,
    Operator,
    Modifier,
    Comparison,
    Parenthesis,
}