using System.Numerics;
using MonadCraft;

namespace RollCraft.Nodes;

internal enum FunctionType
{
    Floor,
    Ceil,
    Round,
    Min,
    Max,
    Abs,
    Sqrt
}

internal sealed class Function<TNumber> : DiceExpression<TNumber> where TNumber : INumber<TNumber>
{
    internal FunctionType FunctionType { get; }
    internal DiceExpression<TNumber>[] Arguments { get; }

    internal Function(FunctionType functionType, params DiceExpression<TNumber>[] arguments)
    {
        FunctionType = functionType;
        Arguments = arguments;
    }

    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller)
    {
        var evaluatedArgs = new TNumber[Arguments.Length];
        var rolls = new List<DiceRoll>();

        for (var i = 0; i < Arguments.Length; i++)
        {
            var argResult = Arguments[i].EvaluateNode(roller);
            if (argResult.IsFailure)
            {
                return argResult;
            }

            evaluatedArgs[i] = argResult.Value.Result;
            rolls.AddRange(argResult.Value.Rolls);
        }

        var result = EvaluateFunction(evaluatedArgs);
        if (result.IsFailure)
        {
            return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Failure(result.Error);
        }

        return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Success((result.Value, rolls));
    }

    internal override Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)> EvaluateNode(IRoller roller, IReadOnlyDictionary<string, TNumber> variables)
    {
        var evaluatedArgs = new TNumber[Arguments.Length];
        var rolls = new List<DiceRoll>();

        for (var i = 0; i < Arguments.Length; i++)
        {
            var argResult = Arguments[i].EvaluateNode(roller, variables);
            if (argResult.IsFailure)
            {
                return argResult;
            }

            evaluatedArgs[i] = argResult.Value.Result;
            rolls.AddRange(argResult.Value.Rolls);
        }

        var result = EvaluateFunction(evaluatedArgs);
        if (result.IsFailure)
        {
            return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Failure(result.Error);
        }

        return Result<IRollError, (TNumber Result, List<DiceRoll> Rolls)>.Success((result.Value, rolls));
    }

    private Result<IRollError, TNumber> EvaluateFunction(TNumber[] args)
    {
        return FunctionType switch
        {
            FunctionType.Floor => EvaluateFloor(args[0]),
            FunctionType.Ceil => EvaluateCeil(args[0]),
            FunctionType.Round => EvaluateRound(args[0]),
            FunctionType.Min => EvaluateMin(args),
            FunctionType.Max => EvaluateMax(args),
            FunctionType.Abs => EvaluateAbs(args[0]),
            FunctionType.Sqrt => EvaluateSqrt(args[0]),
            _ => new EvaluatorError("Evaluator.UnknownFunction", $"Unknown function type: {FunctionType}")
        };
    }

    private static Result<IRollError, TNumber> EvaluateFloor(TNumber value)
    {
        if (TNumber.IsInteger(value))
        {
            return Result<IRollError, TNumber>.Success(value);
        }
        
        // Use decimal-specific path for full precision
        if (typeof(TNumber) == typeof(decimal))
        {
            var decimalValue = decimal.CreateChecked(value);
            var floored = Math.Floor(decimalValue);
            return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(floored));
        }
        
        var doubleValue = double.CreateChecked(value);
        var flooredDouble = Math.Floor(doubleValue);
        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(flooredDouble));
    }

    private static Result<IRollError, TNumber> EvaluateCeil(TNumber value)
    {
        if (TNumber.IsInteger(value))
        {
            return Result<IRollError, TNumber>.Success(value);
        }
        
        // Use decimal-specific path for full precision
        if (typeof(TNumber) == typeof(decimal))
        {
            var decimalValue = decimal.CreateChecked(value);
            var ceiled = Math.Ceiling(decimalValue);
            return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(ceiled));
        }
        
        var doubleValue = double.CreateChecked(value);
        var ceiledDouble = Math.Ceiling(doubleValue);
        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(ceiledDouble));
    }

    private static Result<IRollError, TNumber> EvaluateRound(TNumber value)
    {
        if (TNumber.IsInteger(value))
        {
            return Result<IRollError, TNumber>.Success(value);
        }
        
        // Use decimal-specific path for full precision
        if (typeof(TNumber) == typeof(decimal))
        {
            var decimalValue = decimal.CreateChecked(value);
            var rounded = Math.Round(decimalValue);
            return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(rounded));
        }
        
        var doubleValue = double.CreateChecked(value);
        var roundedDouble = Math.Round(doubleValue);
        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(roundedDouble));
    }

    private static Result<IRollError, TNumber> EvaluateMin(TNumber[] args)
    {
        if (args.Length < 2)
        {
            return new EvaluatorError("Evaluator.FunctionError", "min() requires at least 2 arguments");
        }

        var min = args[0];
        for (var i = 1; i < args.Length; i++)
        {
            if (args[i] < min)
            {
                min = args[i];
            }
        }

        return Result<IRollError, TNumber>.Success(min);
    }

    private static Result<IRollError, TNumber> EvaluateMax(TNumber[] args)
    {
        if (args.Length < 2)
        {
            return new EvaluatorError("Evaluator.FunctionError", "max() requires at least 2 arguments");
        }

        var max = args[0];
        for (var i = 1; i < args.Length; i++)
        {
            if (args[i] > max)
            {
                max = args[i];
            }
        }

        return Result<IRollError, TNumber>.Success(max);
    }

    private static Result<IRollError, TNumber> EvaluateAbs(TNumber value)
    {
        return Result<IRollError, TNumber>.Success(TNumber.Abs(value));
    }

    private static Result<IRollError, TNumber> EvaluateSqrt(TNumber value)
    {
        if (value < TNumber.Zero)
        {
            return new EvaluatorError("Evaluator.FunctionError", "sqrt() cannot be applied to negative numbers");
        }

        // Use decimal-specific path for better precision (note: still limited by decimal's capabilities)
        if (typeof(TNumber) == typeof(decimal))
        {
            var decimalValue = decimal.CreateChecked(value);
            // Decimal doesn't have native sqrt, use Newton-Raphson for better precision than double conversion
            var result = DecimalSqrt(decimalValue);
            return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(result));
        }

        var doubleValue = double.CreateChecked(value);
        var sqrtResult = Math.Sqrt(doubleValue);

        if (TNumber.IsInteger(value))
        {
            // Integer sqrt returns truncated value
            return Result<IRollError, TNumber>.Success(TNumber.CreateTruncating(sqrtResult));
        }

        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(sqrtResult));
    }
    
    private static decimal DecimalSqrt(decimal value)
    {
        if (value == 0m)
        {
            return 0m;
        }
        
        // Newton-Raphson method for decimal sqrt
        var guess = (decimal)Math.Sqrt((double)value); // Initial guess from double
        var epsilon = 0.0000000000000000000000000001m; // Precision threshold
        
        for (var i = 0; i < 100; i++) // Max iterations to prevent infinite loop
        {
            var newGuess = (guess + value / guess) / 2m;
            if (Math.Abs(newGuess - guess) < epsilon)
            {
                return newGuess;
            }
            guess = newGuess;
        }
        
        return guess;
    }

    public override string ToString()
    {
        var funcName = FunctionType.ToString().ToUpperInvariant();
        var args = string.Join(", ", Arguments.Select(a => a.ToString()));
        return $"{funcName}({args})";
    }
}