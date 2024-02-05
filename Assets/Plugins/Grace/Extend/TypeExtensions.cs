using System;

namespace Grace.Extend
{
    public static partial class TypeExtensions
    {
        public static bool GetAttribute<TAttributeType>(this Type type, out TAttributeType foundAttribute)
        {
            foreach (Attribute attribute in type.GetCustomAttributes(false))
            {
                if (attribute is TAttributeType injectionAttribute)
                {
                    foundAttribute = injectionAttribute;
                    return true;
                }
            }

            foundAttribute = default;
            return false;
        }
    }
}