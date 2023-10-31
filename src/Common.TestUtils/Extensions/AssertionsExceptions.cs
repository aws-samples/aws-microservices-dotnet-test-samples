using System.Reflection;
using NUnit.Framework;

namespace Common.TestUtils.Extensions;

public static class AssertionsExtensions
{
    public static void AssertEqualTo<TActual, TExpected>(this TActual actual, TExpected expectedAnonymous)
        where TActual : notnull
        where TExpected : notnull
    {
        var expectedType = expectedAnonymous.GetType();
        var actualType = actual.GetType();
        var result = (TExpected)TransformRealToAnonymous(actual, actualType, expectedType);

        Assert.That(result, Is.EqualTo(expectedAnonymous));
    }

    public static void AssertEqualTo<TActual, TExpected>(this IEnumerable<TActual> actual,
        IEnumerable<TExpected> expectedAnonymous)
    {
        var expectedType = expectedAnonymous.First()!.GetType();

        var transformedResult = new List<TExpected>();
        foreach (var item in actual)
        {
            var actualType = item!.GetType();
            var result = (TExpected)TransformRealToAnonymous(item, actualType, expectedType);
            transformedResult.Add(result);
        }

        Assert.That(transformedResult, Is.EqualTo(expectedAnonymous));
    }

    private static object TransformRealToAnonymous(object instance, Type realType, Type anonymousType)
    {
        var ctor = anonymousType.GetConstructors().Single();
        var constructorParameters = ctor.GetParameters()
            .Select(parameter => GetValue(instance, realType, parameter))
            .ToArray();
        return ctor.Invoke(constructorParameters);
    }

    private static object GetValue(object instance, Type instanceType, ParameterInfo parameterInfo)
    {
        var actualProperty = instanceType.GetProperty(parameterInfo.Name!);
        var actualPropertyValue = actualProperty!.GetValue(instance);
        if (!parameterInfo.ParameterType.IsAnonymousType()) return actualPropertyValue!;

        return TransformRealToAnonymous(actualPropertyValue!, actualProperty.PropertyType, parameterInfo.ParameterType);
    }
}