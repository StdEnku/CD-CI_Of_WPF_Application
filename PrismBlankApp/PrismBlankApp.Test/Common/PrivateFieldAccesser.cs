using System.Diagnostics;
using System.Reflection;

namespace PrismBlankApp.Test.Common
{
    internal static class PrivateFieldAccesser
    {
        internal static void SetPrivateField<T>(T obj, string fieldName, object value)
        {
            var typeInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(typeInfo is not null);
            typeInfo.SetValue(obj, value);
        }

        internal static object? GetPrivateField<T>(T obj, string fieldName)
        {
            var typeInfo = typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            Debug.Assert(typeInfo is not null);
            var result = typeInfo.GetValue(obj);
            return result;
        }
    }
}