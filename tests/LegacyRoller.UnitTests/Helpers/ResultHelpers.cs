using LitePrimitives;
using TUnit.Assertions.Exceptions;

namespace LegacyRoller.UnitTests.Helpers;

public static class ResultHelpers
{
    public static T GetSuccessValue<T>(this Result<T> result)
    {
        return result.Match(
            success: value => value,
            failure: _ => throw new AssertionException("Result was not a success"));
    }
}