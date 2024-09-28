namespace LegacyRoller;

internal class Unary : DiceExpression
{
    internal DiceExpression Expression { get; }

    internal Unary(DiceExpression expression)
    {
        Expression = expression;
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}