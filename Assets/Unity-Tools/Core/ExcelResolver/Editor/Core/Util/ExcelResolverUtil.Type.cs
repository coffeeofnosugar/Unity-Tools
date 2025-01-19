using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal static partial class ExcelResolverUtil
    {
        /// <summary>
        /// 类型缓存，避免重复反射查找
        /// StringComparer.OrdinalIgnoreCase: 忽略键的大小写
        /// </summary>
        static readonly Dictionary<string, Type> TypeCache = new(StringComparer.OrdinalIgnoreCase);
        
        /// <summary>
        /// 通过类名(含命名空间)获取 Type，并缓存
        /// </summary>
        internal static Type GetOrCacheTypeByName(string typeName)
        {
            typeName = typeName.Trim();

            // 如果缓存中存在，直接返回
            if (TypeCache.TryGetValue(typeName, out var result))
            {
                return result;
            }

            // 尝试从 Tools.ExcelResolver 命名空间下查找
            string fullTypeName = $"Tools.ExcelResolver.{typeName}";
            result = Type.GetType(fullTypeName, false, true) ?? GetTypeFromNecessaryAssemblies(fullTypeName);
            if (result != null)
            {
                return result;
            }
            
            throw new ArgumentException($"Unsupported type: {typeName}");
        }
        
        /// <summary>
        /// 从所有程序集中查找类型
        /// </summary>
        /// <param name="fullTypeName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static Type GetTypeFromNecessaryAssemblies(string fullTypeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.GetName().Name 
                    is "UnityEngine"
                    or "Assembly-CSharp"
                    or "Assembly-CSharp-firstpass"
                    or "Assembly-CSharp-Editor"
                    or "Assembly-CSharp-Editor-firstpass");
            
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(fullTypeName);
                if (assembly.GetName().Name is "Assembly-CSharp-Editor" or "Assembly-CSharp-Editor-firstpass")
                {
                    throw new ArgumentException($"不支持Editor目录下的'{fullTypeName}'类型");
                }
                if (type != null)
                {
                    TypeCache[fullTypeName] = type;
                    return type;
                }
            }

            return null;
        }

        internal static void Dispose()
        {
            TypeCache.Clear();
        }
        
        /*
        /// <summary>
        /// 参考：https://learn.microsoft.com/zh-cn/dotnet/api/system.type.gettype?view=net-8.0
        /// </summary>
        /// <param name="typeText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        static Type GetType(string typeText)
        {
            // 首先尝试使用Type.GetType
            Type type = Type.GetType($"System.{typeText}", false, true);
            if (type != null) return type;

            // 如果失败，尝试在UnityEngine命名空间下查找
            // 参数一："[命名空间.类型名], [程序集名]"
            // 参数二：是否抛出异常
            // 参数三：是否区分大小写
            type = Type.GetType($"UnityEngine.{typeText}, UnityEngine", false, true);
            if (type != null) return type;
            
            throw new ArgumentException($"Unsupported type: {typeText}");
        }
        */
    }
}