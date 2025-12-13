namespace RollCraft;

/// <summary>
/// Represents an error that occurred during parsing of a dice expression string.
/// </summary>
/// <remarks>
/// Parser errors include syntax errors, unexpected tokens, and invalid expression structures.
/// The <see cref="Position"/> property indicates where in the input string the error was detected.
/// </remarks>
public readonly record struct ParserError : IRollError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ParserError"/> struct.
    /// </summary>
    /// <param name="errorCode">The unique error code identifying the type of error.</param>
    /// <param name="message">A human-readable description of the error.</param>
    /// <param name="position">The character position in the input string where the error was detected.</param>
    public ParserError(string errorCode, string message, int position)
    {
        ErrorCode = errorCode;
        Message = message;
        Position = position;
    }
    
    /// <inheritdoc />
    public string ErrorCode { get; }
    
    /// <inheritdoc />
    public string Message { get; }
    
    /// <summary>
    /// Gets the character position (0-based) in the input string where the error was detected.
    /// </summary>
    public int Position { get; }
}