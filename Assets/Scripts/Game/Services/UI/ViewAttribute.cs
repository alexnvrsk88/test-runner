using System;

namespace Runner.Game.Services.UI
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ViewAttribute : Attribute
    {
        public readonly string Name;
        public readonly ViewLayer Layer;

        public ViewAttribute(string name, ViewLayer layer = ViewLayer.Default)
        {
            Name = name;
            Layer = layer;
        }
    }
}