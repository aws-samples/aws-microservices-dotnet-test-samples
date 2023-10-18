using System.Runtime.CompilerServices;

namespace Common.TestUtils.Extensions;

public static class ReflectionExtensions
{
    public static bool IsAnonymousType(this Type type)
    {
        var hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Any();
        var nameContainsAnonymousType = type.FullName!.Contains("AnonymousType");

        var isAnonymousType = hasCompilerGeneratedAttribute && nameContainsAnonymousType;

        return isAnonymousType;
    }
}