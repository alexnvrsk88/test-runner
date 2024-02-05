using System;

namespace Runner.Game
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class InitializationAttribute : Attribute
    {
        public readonly int Order;

        public InitializationAttribute(int order)
        {
            Order = order;
        }
    }
}