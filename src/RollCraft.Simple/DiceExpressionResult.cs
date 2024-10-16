namespace RollCraft.Simple;

public sealed class DiceExpressionResult
{
    public DiceExpressionResult(int result, List<DiceRoll>? rolls = null)
    {
        Result = result;
        Rolls = rolls ?? [];
    }
    
    public int Result { get; set; }
    public List<DiceRoll> Rolls { get; set; }
}