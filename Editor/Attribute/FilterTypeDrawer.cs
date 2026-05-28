using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Scream.UniEJ.Common;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Scream.UniEJ.Editor
{

    [CustomPropertyDrawer(typeof(FilterTypeAttribute))]
    public sealed class FilterTypeDrawer : PropertyDrawer
    {
        private const string TypeNameFieldName = "_assemblyQualifiedTypeName";

        private FilterTypeAttribute FilterTypeAttribute => (FilterTypeAttribute)attribute;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var typeNameProperty = GetTypeNameProperty(property);
            if (typeNameProperty == null)
            {
                EditorGUI.HelpBox(position, "FilterType supports string or FilterTypeReference<T> fields.", MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            string error;
            var types = GetFilteredTypes(property.serializedObject.targetObject, FilterTypeAttribute.MethodName, out error);
            if (!string.IsNullOrEmpty(error))
            {
                EditorGUI.HelpBox(position, error, MessageType.Error);
                EditorGUI.EndProperty();
                return;
            }

            DrawTypePopup(position, typeNameProperty, label, types);
            EditorGUI.EndProperty();
        }

        private static SerializedProperty GetTypeNameProperty(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                return property;
            }

            var typeNameProperty = property.FindPropertyRelative(TypeNameFieldName);
            if (typeNameProperty != null && typeNameProperty.propertyType == SerializedPropertyType.String)
            {
                return typeNameProperty;
            }

            return null;
        }

        private static void DrawTypePopup(
            Rect position,
            SerializedProperty typeNameProperty,
            GUIContent label,
            IReadOnlyList<Type> types)
        {
            var options = BuildOptions(typeNameProperty.stringValue, types);
            var selectedIndex = Mathf.Max(0, options.FindIndex(x => x.AssemblyQualifiedName == typeNameProperty.stringValue));
            var displayOptions = options.Select(x => x.DisplayName).ToArray();

            EditorGUI.showMixedValue = typeNameProperty.hasMultipleDifferentValues;
            var newIndex = EditorGUI.Popup(position, label.text, selectedIndex, displayOptions);
            EditorGUI.showMixedValue = false;

            if (newIndex != selectedIndex)
            {
                typeNameProperty.stringValue = options[newIndex].AssemblyQualifiedName;
            }
        }

        private static List<TypeOption> BuildOptions(string currentTypeName, IReadOnlyList<Type> types)
        {
            var options = new List<TypeOption>
        {
            new TypeOption("None", string.Empty)
        };

            foreach (var type in types)
            {
                options.Add(new TypeOption(GetDisplayName(type), type.AssemblyQualifiedName));
            }

            if (!string.IsNullOrEmpty(currentTypeName) && options.All(x => x.AssemblyQualifiedName != currentTypeName))
            {
                options.Insert(1, new TypeOption($"Missing ({GetShortTypeName(currentTypeName)})", currentTypeName));
            }

            return options;
        }

        private static string GetDisplayName(Type type)
        {
            return string.IsNullOrEmpty(type.Namespace)
                ? type.Name
                : $"{type.Namespace}.{type.Name}";
        }

        private static string GetShortTypeName(string assemblyQualifiedTypeName)
        {
            var separatorIndex = assemblyQualifiedTypeName.IndexOf(",", StringComparison.Ordinal);
            return separatorIndex < 0
                ? assemblyQualifiedTypeName
                : assemblyQualifiedTypeName.Substring(0, separatorIndex);
        }

        private static IReadOnlyList<Type> GetFilteredTypes(Object target, string methodName, out string error)
        {
            error = null;

            if (target == null)
            {
                error = "FilterType target is missing.";
                return Array.Empty<Type>();
            }

            var method = GetFilterMethod(target.GetType(), methodName);
            if (method == null)
            {
                error = $"FilterType method '{methodName}' was not found.";
                return Array.Empty<Type>();
            }

            if (method.GetParameters().Length > 0)
            {
                error = $"FilterType method '{methodName}' must not have parameters.";
                return Array.Empty<Type>();
            }

            object result;
            try
            {
                result = method.Invoke(method.IsStatic ? null : target, null);
            }
            catch (TargetInvocationException exception)
            {
                error = exception.InnerException?.Message ?? exception.Message;
                return Array.Empty<Type>();
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return Array.Empty<Type>();
            }

            var types = GetTypesFromResult(result)
                .Where(x => x != null)
                .Where(x => !x.IsAbstract)
                .Where(x => !x.IsInterface)
                .OrderBy(GetDisplayName)
                .ToArray();

            return types;
        }

        private static MethodInfo GetFilterMethod(Type type, string methodName)
        {
            const BindingFlags flags =
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly;

            for (var current = type; current != null; current = current.BaseType)
            {
                var method = current.GetMethod(methodName, flags);
                if (method != null)
                {
                    return method;
                }
            }

            return null;
        }

        private static IEnumerable<Type> GetTypesFromResult(object result)
        {
            if (result == null)
            {
                return Array.Empty<Type>();
            }

            if (result is IEnumerable<Type> typeEnumerable)
            {
                return typeEnumerable;
            }

            if (result is IEnumerable enumerable)
            {
                return enumerable.OfType<Type>();
            }

            return Array.Empty<Type>();
        }

        private readonly struct TypeOption
        {
            public readonly string DisplayName;
            public readonly string AssemblyQualifiedName;

            public TypeOption(string displayName, string assemblyQualifiedName)
            {
                DisplayName = displayName;
                AssemblyQualifiedName = assemblyQualifiedName;
            }
        }
    }

}