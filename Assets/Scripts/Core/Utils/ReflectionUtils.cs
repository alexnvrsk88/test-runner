using System;
using System.Linq;

namespace Runner.Core.Utils
{
    public static class ReflectionUtils
    {
        public static Type GetType(string className)
        {
            Type type = null;
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.DefinedTypes.Any(dt => dt.Name.Contains(className)) && !asm.FullName.Contains("Unity"));
            if (assembly == null)
            {
                return null;
            }

            type = assembly.DefinedTypes.FirstOrDefault(dt => dt.Name.Equals(className));
            return type;
        }

        public static T Create<T>(string className)
        {
            T instance;
            var type = GetType(className);
            instance = (T)Activator.CreateInstance(type);

            return instance;
        }

        public static T Create<T>(string className, params object[] arguments)
        {
            T instance;
            var type = GetType(className);
            instance = (T)Activator.CreateInstance(type, arguments);

            return instance;
        }
    }
}