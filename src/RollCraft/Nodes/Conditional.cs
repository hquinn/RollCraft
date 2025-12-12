using System.Numerics;
using MonadCraft;

namespace RollCraft.Nodes;

internal sealed class Conditional<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal DiceExpression<TNumber> Left { get; }
    internal ConditionalOperator Operator { get; }
    internal DiceExpression<TNumber> Right { get; }
    internal DiceExpression<TNumber> TrueExpression { get; }
    internal DiceExpression<TNumber> FalseExpression { get; }

    internal Conditional(
        DiceExpression<TNumber> left,
        ConditionalOperator @operator,
        DiceExpression<TNumber> right,
        DiceExpression<TNumber> trueExpression,
        DiceExpression<TNumber> falseExpression)
    {
        Left = left;
        Operator = @operator;
        Right = right;
        TrueExpression = trueExpression;
        FalseExpression = falseExpression;
    }

    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        var leftResult = Left.EvaluateNode(roller);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        var rightResult = Right.EvaluateNode(roller);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        var conditionMet = EvaluateCondition(leftResult.Value.Result, rightResult.Value.Result);
        
        var rolls = leftResult.Value.Rolls;
        rolls.AddRange(rightResult.Value.Rolls);

        var branchResult = conditionMet 
            ? TrueExpression.EvaluateNode(roller) 
            : FalseExpression.EvaluateNode(roller);

        if (branchResult.IsFailure)
        {
            return branchResult;
        }

        rolls.AddRange(branchResult.Value.Rolls);
        return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Success((branchResult.Value.Result, rolls));
    }
    
    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        var leftResult = Left.EvaluateNode(roller, variables);
        if (leftResult.IsFailure)
        {
            return leftResult;
        }

        var rightResult = Right.EvaluateNode(roller, variables);
        if (rightResult.IsFailure)
        {
            return rightResult;
        }

        var conditionMet = EvaluateCondition(leftResult.Value.Result, rightResult.Value.Result);
        
        var rolls = leftResult.Value.Rolls;
        rolls.AddRange(rightResult.Value.Rolls);

        var branchResult = conditionMet 
            ? TrueExpression.EvaluateNode(roller, variables) 
            : FalseExpression.EvaluateNode(roller, variables);

        if (branchResult.IsFailure)
        {
            return branchResult;
        }

        rolls.AddRange(branchResult.Value.Rolls);
        return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Success((branchResult.Value.Result, rolls));
    }

    private bool EvaluateCondition(TNumber left, TNumber right)
    {
        return Operator switch
        {
            ConditionalOperator.Equal => left == right,
            ConditionalOperator.NotEqual => left != right,
            ConditionalOperator.GreaterThan => left > right,
            ConditionalOperator.GreaterThanEqual => left >= right,
            ConditionalOperator.LessThan => left < right,
            ConditionalOperator.LessThanEqual => left <= right,
            _ => false
        };
    }
    
    public override string ToString()
    {
        var op = Operator switch
        {
            ConditionalOperator.Equal => "=",
            ConditionalOperator.NotEqual => "<>",
            ConditionalOperator.GreaterThan => ">",
            ConditionalOperator.GreaterThanEqual => ">=",
            ConditionalOperator.LessThan => "<",
            ConditionalOperator.LessThanEqual => "<=",
            _ => "?"
        };
        return $"IF({Left} {op} {Right}, {TrueExpression}, {FalseExpression})";
    }
}

internal enum ConditionalOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanEqual,
    LessThan,
    LessThanEqual
}
