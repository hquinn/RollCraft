using System.Numerics;
using MonadCraft;
using RollCraft.Rollers;

namespace RollCraft;

/// <summary>
/// Evaluates dice expressions and returns results. Provides multiple factory methods for different rolling strategies.
/// </summary>
/// <typeparam name="TNumber">The numeric type for expression values. Supported types are <see cref="short"/>, <see cref="int"/>, <see cref="long"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>.</typeparam>
/// <remarks>
/// <para>
/// Create an evaluator using one of the static factory methods:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="CreateRandom"/> - Uses random dice rolls</description></item>
/// <item><description><see cref="CreateSeededRandom"/> - Uses a seeded random number generator for reproducible results</description></item>
/// <item><description><see cref="CreateMinimum"/> - Always rolls 1 on every die</description></item>
/// <item><description><see cref="CreateMaximum"/> - Always rolls the maximum value on every die</description></item>
/// <item><description><see cref="CreateFixedAverage"/> - Rolls the average value for each die (rounded up)</description></item>
/// <item><description><see cref="CreateCustom"/> - Uses a custom <see cref="IRoller"/> implementation</description></item>
/// </list>
/// </remarks>
/// <example>
/// <code>
/// var evaluator = DiceExpressionEvaluator&lt;int&gt;.CreateRandom();
/// var result = evaluator.Evaluate("2d6+5");
/// if (result.IsSuccess)
/// {
///     Console.WriteLine($"Result: {result.Value.Result}");
/// }
/// </code>
/// </example>
public class DiceExpressionEvaluator<TNumber> 
    where TNumber : INumber<TNumber>
{
    private readonly IRoller _roller;
    
    private DiceExpressionEvaluator(IRoller roller)
    {
        _roller = roller;
    }
    
    /// <summary>
    /// Creates an evaluator with a custom roller implementation.
    /// </summary>
    /// <param name="roller">The custom <see cref="IRoller"/> implementation to use for dice rolls.</param>
    /// <returns>A new <see cref="DiceExpressionEvaluator{TNumber}"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="roller"/> is null.</exception>
    public static DiceExpressionEvaluator<TNumber> CreateCustom(IRoller roller)
    {
        ArgumentNullException.ThrowIfNull(roller);
        return new DiceExpressionEvaluator<TNumber>(roller);
    }

    /// <summary>
    /// Creates an evaluator that uses a random number generator for dice rolls.
    /// </summary>
    /// <returns>A new <see cref="DiceExpressionEvaluator{TNumber}"/> instance.</returns>
    public static DiceExpressionEvaluator<TNumber> CreateRandom()
    {
        return new DiceExpressionEvaluator<TNumber>(new RandomRoller());
    }

    /// <summary>
    /// Creates an evaluator that uses a seeded random number generator for reproducible dice rolls.
    /// </summary>
    /// <param name="seed">The seed value for the random number generator.</param>
    /// <returns>A new <see cref="DiceExpressionEvaluator{TNumber}"/> instance.</returns>
    public static DiceExpressionEvaluator<TNumber> CreateSeededRandom(int seed)
    {
        return new DiceExpressionEvaluator<TNumber>(new SeededRandomRoller(seed));
    }
    
    /// <summary>
    /// Creates an evaluator that always rolls the minimum value (1) on every die.
    /// </summary>
    /// <returns>A new <see cref="DiceExpressionEvaluator{TNumber}"/> instance.</returns>
    public static DiceExpressionEvaluator<TNumber> CreateMinimum()
    {
        return new DiceExpressionEvaluator<TNumber>(new MinimumRoller());
    }

    /// <summary>
    /// Creates an evaluator that always rolls the maximum value on every die.
    /// </summary>
    /// <returns>A new <see cref="DiceExpressionEvaluator{TNumber}"/> instance.</returns>
    public static DiceExpressionEvaluator<TNumber> CreateMaximum()
    {
        return new DiceExpressionEvaluator<TNumber>(new MaximumRoller());
    }

    /// <summary>
    /// Creates an evaluator that always rolls the average value for each die (rounded up).
    /// </summary>
    /// <returns>A new <see cref="DiceExpressionEvaluator{TNumber}"/> instance.</returns>
    /// <remarks>
    /// The average is calculated as <c>(dieSize / 2) + 1</c>. For a d6, this returns 4.
    /// </remarks>
    public static DiceExpressionEvaluator<TNumber> CreateFixedAverage()
    {
        return new DiceExpressionEvaluator<TNumber>(new FixedAverageRoller());
    }

    /// <summary>
    /// Evaluates a parsed dice expression result.
    /// </summary>
    /// <param name="expression">The result from <see cref="DiceExpressionParser.Parse{TNumber}"/>.</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing the evaluation result or an error if parsing or evaluation failed.
    /// </returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(Result<IRollError, DiceExpression<TNumber>> expression)
    {
        if (expression.IsFailure)
        {
            return Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(expression.Error);
        }
        
        return Evaluate(expression.Value);
    }
    
    /// <summary>
    /// Evaluates a parsed dice expression result with variable substitution.
    /// </summary>
    /// <param name="expression">The result from <see cref="DiceExpressionParser.Parse{TNumber}"/>.</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing the evaluation result or an error if parsing or evaluation failed.
    /// </returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(Result<IRollError, DiceExpression<TNumber>> expression, IReadOnlyDictionary<string, TNumber> variables)
    {
        if (expression.IsFailure)
        {
            return Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(expression.Error);
        }
        
        return Evaluate(expression.Value, variables);
    }

    /// <summary>
    /// Evaluates a parsed dice expression.
    /// </summary>
    /// <param name="expression">The parsed <see cref="DiceExpression{TNumber}"/> to evaluate.</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing the evaluation result or an error if evaluation failed.
    /// </returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(DiceExpression<TNumber> expression)
    {
        return expression.Evaluate(_roller);
    }
    
    /// <summary>
    /// Evaluates a parsed dice expression with variable substitution.
    /// </summary>
    /// <param name="expression">The parsed <see cref="DiceExpression{TNumber}"/> to evaluate.</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing the evaluation result or an error if evaluation failed.
    /// </returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(DiceExpression<TNumber> expression, IReadOnlyDictionary<string, TNumber> variables)
    {
        return expression.Evaluate(_roller, variables);
    }

    /// <summary>
    /// Parses and evaluates a dice expression string.
    /// </summary>
    /// <param name="expression">The dice expression string to parse and evaluate (e.g., "2d6+5").</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing the evaluation result or an error if parsing or evaluation failed.
    /// </returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(string expression)
    {
        var parsedExpression = DiceExpressionParser.Parse<TNumber>(expression);

        if (parsedExpression.IsFailure)
        {
            return Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(parsedExpression.Error);
        }

        return parsedExpression.Value.Evaluate(_roller);
    }
    
    /// <summary>
    /// Parses and evaluates a dice expression string with variable substitution.
    /// </summary>
    /// <param name="expression">The dice expression string to parse and evaluate (e.g., "1d20+[STR]").</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <returns>
    /// A <see cref="Result{TError, TValue}"/> containing the evaluation result or an error if parsing or evaluation failed.
    /// </returns>
    /// <example>
    /// <code>
    /// var evaluator = DiceExpressionEvaluator&lt;int&gt;.CreateRandom();
    /// var variables = new Dictionary&lt;string, int&gt; { ["STR"] = 5, ["DEX"] = 3 };
    /// var result = evaluator.Evaluate("1d20+[STR]", variables);
    /// </code>
    /// </example>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>> Evaluate(string expression, IReadOnlyDictionary<string, TNumber> variables)
    {
        var parsedExpression = DiceExpressionParser.Parse<TNumber>(expression);

        if (parsedExpression.IsFailure)
        {
            return Result<IRollError, DiceExpressionResult<IRollError, TNumber>>.Failure(parsedExpression.Error);
        }

        return parsedExpression.Value.Evaluate(_roller, variables);
    }

    /// <summary>
    /// Evaluates a parsed dice expression multiple times.
    /// </summary>
    /// <param name="expression">The parsed <see cref="DiceExpression{TNumber}"/> to evaluate.</param>
    /// <param name="repeatCount">The number of times to evaluate the expression.</param>
    /// <returns>An array of results, one for each evaluation.</returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[] Evaluate(DiceExpression<TNumber> expression, ushort repeatCount)
    {
        var results = new Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[repeatCount];
        
        for (var i = 0; i < repeatCount; i++)
        {
            results[i] = Evaluate(expression);
        }
        
        return results;
    }
    
    /// <summary>
    /// Evaluates a parsed dice expression multiple times with variable substitution.
    /// </summary>
    /// <param name="expression">The parsed <see cref="DiceExpression{TNumber}"/> to evaluate.</param>
    /// <param name="repeatCount">The number of times to evaluate the expression.</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <returns>An array of results, one for each evaluation.</returns>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[] Evaluate(DiceExpression<TNumber> expression, ushort repeatCount, IReadOnlyDictionary<string, TNumber> variables)
    {
        var results = new Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[repeatCount];
        
        for (var i = 0; i < repeatCount; i++)
        {
            results[i] = Evaluate(expression, variables);
        }
        
        return results;
    }

    /// <summary>
    /// Parses and evaluates a dice expression string multiple times.
    /// </summary>
    /// <param name="expression">The dice expression string to parse and evaluate.</param>
    /// <param name="repeatCount">The number of times to evaluate the expression.</param>
    /// <returns>An array of results, one for each evaluation.</returns>
    /// <remarks>
    /// Note: The expression is re-parsed for each evaluation. For better performance with repeated
    /// evaluations, use <see cref="DiceExpressionParser.Parse{TNumber}"/> once and pass the result
    /// to <see cref="Evaluate(DiceExpression{TNumber}, int)"/>.
    /// </remarks>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[] Evaluate(string expression, ushort repeatCount)
    {
        var results = new Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[repeatCount];

        for (var i = 0; i < repeatCount; i++)
        {
            results[i] =  Evaluate(expression);
        }

        return results;
    }
    
    /// <summary>
    /// Parses and evaluates a dice expression string multiple times with variable substitution.
    /// </summary>
    /// <param name="expression">The dice expression string to parse and evaluate.</param>
    /// <param name="repeatCount">The number of times to evaluate the expression.</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <returns>An array of results, one for each evaluation.</returns>
    /// <remarks>
    /// Note: The expression is re-parsed for each evaluation. For better performance with repeated
    /// evaluations, use <see cref="DiceExpressionParser.Parse{TNumber}"/> once and pass the result
    /// to <see cref="Evaluate(DiceExpression{TNumber}, int, IReadOnlyDictionary{string, TNumber})"/>.
    /// </remarks>
    public Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[] Evaluate(string expression, ushort repeatCount, IReadOnlyDictionary<string, TNumber> variables)
    {
        var results = new Result<IRollError, DiceExpressionResult<IRollError, TNumber>>[repeatCount];

        for (var i = 0; i < repeatCount; i++)
        {
            results[i] = Evaluate(expression, variables);
        }

        return results;
    }
    
    /// <summary>
    /// Attempts to evaluate a parsed dice expression using the Try pattern.
    /// </summary>
    /// <param name="expression">The parsed <see cref="DiceExpression{TNumber}"/> to evaluate.</param>
    /// <param name="result">
    /// When this method returns, contains the evaluation result if successful, or <c>null</c> if evaluation failed.
    /// </param>
    /// <returns>
    /// <c>null</c> if evaluation succeeded; otherwise, an <see cref="EvaluatorError"/> describing the failure.
    /// </returns>
    public EvaluatorError? TryEvaluate(DiceExpression<TNumber> expression, out DiceExpressionResult<IRollError, TNumber>? result)
    {
        var evalResult = Evaluate(expression);
        
        if (evalResult.IsSuccess)
        {
            result = evalResult.Value;
            return null;
        }
        
        result = null;
        return (EvaluatorError)evalResult.Error;
    }
    
    /// <summary>
    /// Attempts to evaluate a parsed dice expression with variables using the Try pattern.
    /// </summary>
    /// <param name="expression">The parsed <see cref="DiceExpression{TNumber}"/> to evaluate.</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <param name="result">
    /// When this method returns, contains the evaluation result if successful, or <c>null</c> if evaluation failed.
    /// </param>
    /// <returns>
    /// <c>null</c> if evaluation succeeded; otherwise, an <see cref="EvaluatorError"/> describing the failure.
    /// </returns>
    public EvaluatorError? TryEvaluate(DiceExpression<TNumber> expression, IReadOnlyDictionary<string, TNumber> variables, out DiceExpressionResult<IRollError, TNumber>? result)
    {
        var evalResult = Evaluate(expression, variables);
        
        if (evalResult.IsSuccess)
        {
            result = evalResult.Value;
            return null;
        }
        
        result = null;
        return (EvaluatorError)evalResult.Error;
    }
    
    /// <summary>
    /// Attempts to parse and evaluate a dice expression string using the Try pattern.
    /// </summary>
    /// <param name="expression">The dice expression string to parse and evaluate.</param>
    /// <param name="result">
    /// When this method returns, contains the evaluation result if successful, or <c>null</c> if parsing or evaluation failed.
    /// </param>
    /// <returns>
    /// <c>null</c> if successful; otherwise, an <see cref="IRollError"/> (<see cref="ParserError"/> or <see cref="EvaluatorError"/>)
    /// describing the failure.
    /// </returns>
    public IRollError? TryEvaluate(string expression, out DiceExpressionResult<IRollError, TNumber>? result)
    {
        var evalResult = Evaluate(expression);
        
        if (evalResult.IsSuccess)
        {
            result = evalResult.Value;
            return null;
        }
        
        result = null;
        return evalResult.Error;
    }
    
    /// <summary>
    /// Attempts to parse and evaluate a dice expression string with variables using the Try pattern.
    /// </summary>
    /// <param name="expression">The dice expression string to parse and evaluate.</param>
    /// <param name="variables">A dictionary mapping variable names (case-insensitive) to their values.</param>
    /// <param name="result">
    /// When this method returns, contains the evaluation result if successful, or <c>null</c> if parsing or evaluation failed.
    /// </param>
    /// <returns>
    /// <c>null</c> if successful; otherwise, an <see cref="IRollError"/> (<see cref="ParserError"/> or <see cref="EvaluatorError"/>)
    /// describing the failure.
    /// </returns>
    /// <example>
    /// <code>
    /// var evaluator = DiceExpressionEvaluator&lt;int&gt;.CreateRandom();
    /// var variables = new Dictionary&lt;string, int&gt; { ["STR"] = 5 };
    /// var error = evaluator.TryEvaluate("1d20+[STR]", variables, out var result);
    /// if (error is null)
    /// {
    ///     Console.WriteLine($"Roll result: {result!.Result}");
    /// }
    /// </code>
    /// </example>
    public IRollError? TryEvaluate(string expression, IReadOnlyDictionary<string, TNumber> variables, out DiceExpressionResult<IRollError, TNumber>? result)
    {
        var evalResult = Evaluate(expression, variables);
        
        if (evalResult.IsSuccess)
        {
            result = evalResult.Value;
            return null;
        }
        
        result = null;
        return evalResult.Error;
    }
}