using System.Numerics;
using MonadCraft;

namespace RollCraft;

/// <summary>
/// Base class for all dice expression AST nodes. Represents a parsed dice expression that can be evaluated.
/// </summary>
/// <typeparam name="TNumber">The numeric type for expression values. Must be <see cref="int"/> or <see cref="double"/>.</typeparam>
/// <remarks>
/// <para>
/// This is an abstract base class. Concrete implementations include nodes for dice rolls, arithmetic operations,
/// numbers, variables, conditionals, and functions.
/// </para>
/// <para>
/// Instances are created by <see cref="DiceExpressionParser.Parse{TNumber}"/> and evaluated using
/// <see cref="DiceExpressionEvaluator{TNumber}"/>.
/// </para>
/// </remarks>
public abstract class DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(IRoller roller)
    {
        var result = EvaluateNode(roller);
        
        return result.IsSuccess
            ? Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Success(new DiceExpressionResult<IRollError, TNumber>(result.Value.Result, result.Value.Rolls))
            : Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(result.Error);
    }
    
    internal Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        var result = EvaluateNode(roller, variables);
        
        return result.IsSuccess
            ? Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Success(new DiceExpressionResult<IRollError, TNumber>(result.Value.Result, result.Value.Rolls))
            : Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(result.Error);
    }

    internal abstract Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller);
    
    internal virtual Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        return EvaluateNode(roller);
    }
}