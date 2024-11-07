using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tools.InterfaceReference.Editor
{
    [CustomPropertyDrawer(typeof(InterfaceReference<>))]
    [CustomPropertyDrawer(typeof(InterfaceReference<,>))]
    public class InterfaceReferenceDrawer : PropertyDrawer
    {
        private const string UnderlyingValueFieldName = "underlyingValue";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var underlyingProperty = property.FindPropertyRelative(UnderlyingValueFieldName);
            var args = GetArguments(fieldInfo);

            EditorGUI.BeginProperty(position, label, property);
            var assignedObject = EditorGUI.ObjectField(position, label, underlyingProperty.objectReferenceValue, typeof(UnityEngine.Object), true);
            if (assignedObject != null)
            {
                Object component = null;
                if (assignedObject is GameObject gameObject)
                    component = gameObject.GetComponent(args.InterfaceType);
                else if (args.InterfaceType.IsInstanceOfType(assignedObject))
                    component = assignedObject;

                if (component != null)
                    ValidateAndAssignObject(underlyingProperty, component, component.name, args.InterfaceType.Name);
                else
                {
                    Debug.LogWarning($"Assigned object does not implement required interface '{args.InterfaceType.Name}'");
                    underlyingProperty.objectReferenceValue = null;
                }
            }
            else
                underlyingProperty.objectReferenceValue = null;
            
            EditorGUI.EndProperty();
            InterfaceReferenceUtil.OnGUI(position, underlyingProperty, label, args);
        }

        private static InterfaceArgs GetArguments(FieldInfo fieldInfo)
        {
            Type objectType = null, interfaceType = null;
            Type fieldType = fieldInfo.FieldType;       // InterfaceReference<>

            bool TryGetTypesFromInterfaceReference(Type type, out Type objType, out Type intfType)
            {
                objType = intfType = null;

                if (type?.IsGenericType != true) return false;      // 判断是否为泛型类型（类似List<>、Dictionary<,>）

                var genericType = type.GetGenericTypeDefinition();  // 如果是泛型类型，则获取其泛型参数
                if (genericType == typeof(InterfaceReference<>))    // 如果是InterfaceReference<>，则将其转换成基类InterfaceReference<,>
                    type = type.BaseType;
                if (type?.GetGenericTypeDefinition() == typeof(InterfaceReference<,>))
                {
                    Type[] types = type.GetGenericArguments();
                    intfType = types[0];
                    objType = types[1];
                    return true;
                }
                return false;
            }

            void GetTypesFromList(Type type, out Type objType, out Type intfType)
            {
                objType = intfType = null;

                var listInterface = type.GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IList<>));
                if (listInterface != null)
                {
                    var elementType = listInterface.GetGenericArguments()[0];
                    TryGetTypesFromInterfaceReference(elementType, out objType, out intfType);
                }
            }

            if (!TryGetTypesFromInterfaceReference(fieldType, out objectType, out interfaceType))
            {
                GetTypesFromList(fieldType, out objectType, out interfaceType);
            }

            return new InterfaceArgs(objectType, interfaceType);
        }

        private static void ValidateAndAssignObject(SerializedProperty property, Object targetObject,
            string componentNameOrType, string interfaceName = null)
        {
            if (targetObject != null)
            {
                property.objectReferenceValue = targetObject;
            }
            else
            {
                Debug.LogWarning(@$"The{(interfaceName != null
                    ? $"GameObject '{componentNameOrType}'"
                    : "assigned object")} doesn't have a component ths implements '{componentNameOrType}'.");
                property.objectReferenceValue = null;
            }
        }
    }

    public struct InterfaceArgs
    {
        public readonly Type ObjectType;
        public readonly Type InterfaceType;

        public InterfaceArgs(Type objectType, Type interfaceType)
        {
            Debug.Assert(typeof(Object).IsAssignableFrom(objectType), $"{nameof(objectType)} 必须是 {typeof(Object)} 类型");
            Debug.Assert(interfaceType.IsInterface, $"{nameof(interfaceType)} 必须是接口");
            
            ObjectType = objectType;
            InterfaceType = interfaceType;
        }
    }
}
 