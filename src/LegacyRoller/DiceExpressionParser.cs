using LitePrimitives;

namespace LegacyRoller;

public abstract class DiceExpression
{
}

public class Number : DiceExpression
{
    public double Value { get; }

    public Number(double value)
    {
        Value = value;
    }
    
    public override string ToString()
    {
        return $"NUMBER({Value})";
    }
}

public class Unary : DiceExpression
{
    public DiceExpression Expression { get; }

    public Unary(DiceExpression expression)
    {
        Expression = expression;
    }
    
    public override string ToString()
    {
        return $"UNARY({Expression})";
    }
}

public static class DiceExpressionParser
{
    public static Result<DiceExpression> Parse(string input)
    {
        double number = double.Parse(input);

        if (number < 0)
        {
            return Result<DiceExpression>.Success(new Unary(new Number(-number)));
        }
        
        return Result<DiceExpression>.Success(new Number(number));
    }
}