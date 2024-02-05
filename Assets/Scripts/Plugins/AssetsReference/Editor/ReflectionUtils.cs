using System;

namespace Plugins.AssetsReference.Editor
{
    internal static class ReflectionUtils
    {
        public static T Create<T>(Type type, params Type[] genericArguments)
        {
            if (!type.ContainsGenericParameters)
            {
                var instance = (T)Activator.CreateInstance(type);

                return instance;
            }

            var genericArgs = type.GetGenericArguments();
            if (genericArguments == null || genericArguments.Length == 0 ||
                genericArguments.Length != genericArgs.Length)
            {
                for (var i = 0; i < genericArgs.Length; i++)
                {
                    var arg = genericArgs[i];
                    if (!arg.IsGenericParameter) continue;

                    var constraints = arg.GetGenericParameterConstraints();
                    genericArgs[i] = constraints.Length > 0
                        ? GetCommonBaseClass(constraints)
                        : typeof(object);
                }
            }
            else
            {
                var equal = true;
                for (var i = 0; i < genericArguments.Length; i++)
                {
                    var arg = genericArgs[i];
                    if (arg.IsGenericParameter)
                    {
                        var constraints = arg.GetGenericParameterConstraints();
                        genericArgs[i] = constraints.Length > 0
                            ? GetCommonBaseClass(constraints)
                            : typeof(object);
                    }

                    if (genericArgs[i].IsInterface)
                    {
                        equal = genericArguments[i] == genericArgs[i] ||
                                genericArguments[i].IsAssignableFrom(genericArgs[i]);
                    }
                    else
                    {
                        equal = genericArguments[i] == genericArgs[i] ||
                                genericArguments[i].IsSubclassOf(genericArgs[i]);
                    }

                    if (!equal) break;
                }

                if (equal) genericArgs = genericArguments;
            }

            type = type.MakeGenericType(genericArgs);

            var nonBoxedInstance = Activator.CreateInstance(type);
            var genericInstance = (T)nonBoxedInstance;

            return genericInstance;
        }

        private static Type GetCommonBaseClass(params Type[] types)
        {
            switch (types.Length)
            {
                case 0: return typeof(object);
                case 1: return types[0];
            }

            // Copy the parameter so we can substitute base class types in the array without messing up the caller
            var temp = new Type[types.Length];

            for (var i = 0; i < types.Length; i++)
            {
                temp[i] = types[i];
            }

            var checkPass = false;

            Type tested = null;

            while (!checkPass)
            {
                tested = temp[0];

                checkPass = true;

                for (var i = 1; i < temp.Length; i++)
                {
                    if (tested == temp[i])
                        continue;

                    // If the tested common basetype (current) is the indexed type's base type
                    // then we can continue with the test by making the indexed type to be its base type
                    if (tested == temp[i].BaseType)
                    {
                        temp[i] = temp[i].BaseType;
                        continue;
                    }
                    // If the tested type is the indexed type's base type, then we need to change all indexed types
                    // before the current type (which are all identical) to be that base type and restart this loop

                    if (tested.BaseType == temp[i])
                    {
                        for (var j = 0; j <= i - 1; j++)
                        {
                            temp[j] = temp[j].BaseType;
                        }

                        checkPass = false;
                        break;
                    }
                    // The indexed type and the tested type are not related
                    // So make everything from index 0 up to and including the current indexed type to be their base type
                    // because the common base type must be further back

                    for (var j = 0; j <= i; j++)
                    {
                        temp[j] = temp[j].BaseType;
                    }

                    checkPass = false;
                    break;
                }

                // If execution has reached here and checkPass is true, we have found our common base type, 
                // if checkPass is false, the process starts over with the modified types
            }

            // There's always at least object
            return tested;
        }
    }
}