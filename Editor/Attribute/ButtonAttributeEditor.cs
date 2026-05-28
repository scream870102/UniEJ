using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Scream.UniEJ.Common;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scream.UniEJ.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true, isFallback = true)]
    public sealed class ButtonAttributeEditor : UnityEditor.Editor
    {
        private IReadOnlyList<ButtonMethod> _buttonMethods;

        private void OnEnable()
        {
            _buttonMethods = target == null
                ? Array.Empty<ButtonMethod>()
                : ButtonMethodCache.Get(target.GetType());
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_buttonMethods == null || _buttonMethods.Count == 0)
            {
                return;
            }

            EditorGUILayout.Space();

            foreach (var buttonMethod in _buttonMethods)
            {
                using (new EditorGUI.DisabledScope(!buttonMethod.CanInvoke))
                {
                    if (GUILayout.Button(new GUIContent(buttonMethod.Label, buttonMethod.Tooltip)))
                    {
                        Invoke(buttonMethod);
                    }
                }
            }
        }

        private void Invoke(ButtonMethod buttonMethod)
        {
            if (buttonMethod.Method.IsStatic)
            {
                Invoke(buttonMethod, null);
                return;
            }

            foreach (var targetObject in targets)
            {
                if (targetObject == null)
                {
                    continue;
                }

                Undo.RecordObject(targetObject, $"Invoke {buttonMethod.Label}");
                Invoke(buttonMethod, targetObject);

                if (EditorUtility.IsPersistent(targetObject))
                {
                    EditorUtility.SetDirty(targetObject);
                }
            }
        }

        private static void Invoke(ButtonMethod buttonMethod, Object targetObject)
        {
            try
            {
                buttonMethod.Method.Invoke(targetObject, null);
            }
            catch (TargetInvocationException exception)
            {
                Debug.LogException(exception.InnerException ?? exception, targetObject);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, targetObject);
            }
        }

        private static class ButtonMethodCache
        {
            private const BindingFlags MethodFlags =
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.DeclaredOnly;

            private static readonly Dictionary<Type, IReadOnlyList<ButtonMethod>> Cache =
                new Dictionary<Type, IReadOnlyList<ButtonMethod>>();

            public static IReadOnlyList<ButtonMethod> Get(Type type)
            {
                if (Cache.TryGetValue(type, out var methods))
                {
                    return methods;
                }

                methods = GetButtonMethods(type).ToArray();
                Cache.Add(type, methods);
                return methods;
            }

            private static IEnumerable<ButtonMethod> GetButtonMethods(Type type)
            {
                foreach (var currentType in GetTypeHierarchy(type))
                {
                    foreach (var method in currentType.GetMethods(MethodFlags))
                    {
                        var attribute = method.GetCustomAttribute<ButtonAttribute>(true);
                        if (attribute == null)
                        {
                            continue;
                        }

                        yield return new ButtonMethod(method, attribute);
                    }
                }
            }

            private static IEnumerable<Type> GetTypeHierarchy(Type type)
            {
                var hierarchy = new Stack<Type>();

                for (var current = type; current != null && current != typeof(Object); current = current.BaseType)
                {
                    hierarchy.Push(current);
                }

                while (hierarchy.Count > 0)
                {
                    yield return hierarchy.Pop();
                }
            }
        }

        private sealed class ButtonMethod
        {
            public readonly MethodInfo Method;
            public readonly string Label;
            public readonly string Tooltip;
            public readonly bool CanInvoke;

            public ButtonMethod(MethodInfo method, ButtonAttribute attribute)
            {
                Method = method;
                Label = string.IsNullOrWhiteSpace(attribute.Label)
                    ? ObjectNames.NicifyVariableName(method.Name)
                    : attribute.Label;
                CanInvoke = method.GetParameters().Length == 0 && !method.ContainsGenericParameters;
                Tooltip = CanInvoke
                    ? string.Empty
                    : "Button methods must be parameterless and non-generic.";
            }
        }
    }
}
