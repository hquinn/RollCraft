namespace RollCraft;

/// <summary>
/// Represents an error that occurred during evaluation of a dice expression.
/// </summary>
/// <remarks>
/// Evaluator errors include runtime errors such as undefined variables, division by zero,
/// or invalid function arguments.
/// </remarks>
public readonly record struct EvaluatorError : IRollError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EvaluatorError"/> struct.
    /// </summary>
    /// <param name="errorCode">The unique error code identifying the type of error.</param>
    /// <param name="message">A human-readable description of the error.</param>
    public EvaluatorError(string errorCode, string message)
    {
        ErrorCode = errorCode;
        Message = message;
    }

    /// <inheritdoc />
    public string ErrorCode { get; }
    
    /// <inheritdoc />
    public string Message { get; }
}