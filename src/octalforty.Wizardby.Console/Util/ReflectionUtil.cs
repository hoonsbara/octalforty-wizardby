using System;
using System.Reflection;

namespace octalforty.Wizardby.Console.Util
{
    internal static class ReflectionUtil
    {
        public static bool IsDefined<TAttribute>(MemberInfo element)
        {
            return element.IsDefined(typeof(TAttribute), false);
        }

        public static TAttribute GetCustomAttribute<TAttribute>(MemberInfo element)
            where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(element, typeof(TAttribute));
        }
    }
}
