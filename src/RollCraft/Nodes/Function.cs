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
        
        var doubleValue = double.CreateChecked(value);
        var floored = Math.Floor(doubleValue);
        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(floored));
    }

    private static Result<IRollError, TNumber> EvaluateCeil(TNumber value)
    {
        if (TNumber.IsInteger(value))
        {
            return Result<IRollError, TNumber>.Success(value);
        }
        
        var doubleValue = double.CreateChecked(value);
        var ceiled = Math.Ceiling(doubleValue);
        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(ceiled));
    }

    private static Result<IRollError, TNumber> EvaluateRound(TNumber value)
    {
        if (TNumber.IsInteger(value))
        {
            return Result<IRollError, TNumber>.Success(value);
        }
        
        var doubleValue = double.CreateChecked(value);
        var rounded = Math.Round(doubleValue);
        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(rounded));
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

        var doubleValue = double.CreateChecked(value);
        var result = Math.Sqrt(doubleValue);

        if (TNumber.IsInteger(value))
        {
            // Integer sqrt returns truncated value
            return Result<IRollError, TNumber>.Success(TNumber.CreateTruncating(result));
        }

        return Result<IRollError, TNumber>.Success(TNumber.CreateChecked(result));
    }

    public override string ToString()
    {
        var funcName = FunctionType.ToString().ToUpperInvariant();
        var args = string.Join(", ", Arguments.Select(a => a.ToString()));
        return $"{funcName}({args})";
    }
}