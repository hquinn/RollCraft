namespace RollCraft;

/// <summary>
/// Base interface for all errors that can occur during parsing or evaluation of dice expressions.
/// </summary>
/// <remarks>
/// Implementations include <see cref="ParserError"/> for parsing failures and <see cref="EvaluatorError"/>
/// for evaluation failures.
/// </remarks>
public interface IRollError
{
    /// <summary>
    /// Gets the unique error code identifying the type of error (e.g., "Parsing.UnexpectedToken").
    /// </summary>
    public string ErrorCode { get; }
    
    /// <summary>
    /// Gets a human-readable description of the error.
    /// </summary>
    public string Message { get; }
}