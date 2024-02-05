using System;

namespace Grace.Extend
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InjectionAttribute : Attribute
    {
        public  bool IsSingleton { get; }
        public bool LocateByCombine { get; }
        public Type InterfaceType { get; }

        public InjectionAttribute(bool isSingleton, Type interfaceType = null, bool locateByCombine = true)
        {
            IsSingleton = isSingleton;
            InterfaceType = interfaceType;
            LocateByCombine = locateByCombine;
        }

        public bool HasInterface()
        {
            return InterfaceType != null;
        }
    }
}