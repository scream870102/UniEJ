using System;


namespace Scream.UniEJ.Common
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ButtonAttribute : Attribute
    {
        public string Label { get; }

        public ButtonAttribute()
        {
        }

        public ButtonAttribute(string label)
        {
            Label = label;
        }
    }
}

