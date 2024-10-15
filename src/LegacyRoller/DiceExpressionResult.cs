namespace LegacyRoller;

public sealed class DiceExpressionResult
{
    public DiceExpressionResult(double result, List<DiceRoll>? rolls = null)
    {
        Result = result;
        Rolls = rolls ?? [];
    }
    
    public double Result { get; set; }
    public List<DiceRoll> Rolls { get; set; }
}