using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Scream.UniEJ.Common
{

    [Serializable]
    public class FilterTypeReference<T> where T : class
    {
        [SerializeField, HideInInspector] private string _assemblyQualifiedTypeName;

        public string AssemblyQualifiedTypeName
        {
            get => _assemblyQualifiedTypeName;
            set => _assemblyQualifiedTypeName = value;
        }

        public Type Type
        {
            get => string.IsNullOrEmpty(_assemblyQualifiedTypeName)
                ? null
                : Type.GetType(_assemblyQualifiedTypeName);
            set
            {
                if (value == null)
                {
                    _assemblyQualifiedTypeName = null;
                    return;
                }

                if (!typeof(T).IsAssignableFrom(value))
                {
                    throw new ArgumentException($"{value} needs to implement or inherit {typeof(T)}.");
                }

                if (value.IsAbstract || value.IsInterface)
                {
                    throw new ArgumentException($"{value} needs to be a concrete type.");
                }

                _assemblyQualifiedTypeName = value.AssemblyQualifiedName;
            }
        }

        public T Value => CreateInstance();

        public T CreateInstance()
        {
            var type = Type;
            if (type == null)
            {
                return null;
            }

            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"{type} needs to implement or inherit {typeof(T)}.");
            }

            return (T)CreateInstance(type);
        }

        private static object CreateInstance(Type type)
        {
            var constructor = GetConstructor(type);
            if (constructor == null)
            {
                throw new MissingMethodException(type.FullName, ".ctor");
            }

            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return constructor.Invoke(null);
            }

            var arguments = parameters.Select(GetDefaultValue).ToArray();
            return constructor.Invoke(arguments);
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var parameterlessConstructor = type.GetConstructor(flags, null, Type.EmptyTypes, null);
            if (parameterlessConstructor != null)
            {
                return parameterlessConstructor;
            }

            return type.GetConstructors(flags)
                .Where(x => x.GetParameters().All(parameter => parameter.IsOptional || parameter.HasDefaultValue))
                .OrderBy(x => x.GetParameters().Length)
                .FirstOrDefault();
        }

        private static object GetDefaultValue(ParameterInfo parameter)
        {
            if (parameter.HasDefaultValue)
            {
                return parameter.DefaultValue;
            }

            return parameter.ParameterType.IsValueType
                ? Activator.CreateInstance(parameter.ParameterType)
                : null;
        }

        public static implicit operator T(FilterTypeReference<T> reference)
        {
            return reference?.CreateInstance();
        }
    }
}
