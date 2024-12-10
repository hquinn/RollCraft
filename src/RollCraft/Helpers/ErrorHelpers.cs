using System.Collections.ObjectModel;
using LitePrimitives;

namespace RollCraft.Helpers;

public static class ErrorHelpers
{
    public static Error Create(string code, string message, int position)
    {
        return Error.Default(code, message, ParserPositionMetadata(position));
    }
    
    private static ReadOnlyDictionary<string, object> ParserPositionMetadata(int readerPosition)
    {
        return new ReadOnlyDictionary<string, object>(
            new Dictionary<string, object> { { "Position", readerPosition } });
    }
}