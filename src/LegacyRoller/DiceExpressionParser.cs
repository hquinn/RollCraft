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

public static class DiceExpressionParser
{
    public static DiceExpression Parse(string input)
    {
        double number = double.Parse(input);
        return new Number(number);
    }
}