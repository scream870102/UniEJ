using UnityEngine;


namespace Scream.UniEJ.Common
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public sealed class FilterTypeAttribute : PropertyAttribute
    {
        public string MethodName { get; }

        public FilterTypeAttribute(string methodName)
        {
            MethodName = methodName;
        }
    }
}

