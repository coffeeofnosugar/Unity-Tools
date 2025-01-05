using System;
using System.Collections.Generic;

namespace Tools.ExcelResolver.Editor
{
    internal static partial class ExcelResolverUtil
    {
        /// <summary>
        /// 类型缓存，避免重复反射查找
        /// </summary>
        internal static readonly Dictionary<string, Type> TypeCache = new(StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// 通过类名(含命名空间)获取 Type，并缓存
        /// </summary>
        internal static Type GetOrCacheTypeByName(string typeName)
        {
            if (TypeCache.TryGetValue(typeName, out Type cachedType))
            {
                return cachedType;
            }

            string namespacedType = $"Tools.ExcelResolver.{typeName}";
            Type type = Type.GetType(namespacedType, false, true) ?? GetTypeFromAllAssemblies(namespacedType);

            if (type != null)
            {
                TypeCache[typeName] = type;
            }

            return type;
        }
        
        internal static Type GetTypeFromAllAssemblies(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                {
                    TypeCache[typeName] = type;
                    return type;
                }
            }
            
            throw new ArgumentException($"Unsupported type: {typeName}");
        }

        internal static void Dispose()
        {
            TypeCache.Clear();
        }
    }
}