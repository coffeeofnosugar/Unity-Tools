using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal static class TypeUtil
    {
        
        internal static TType GetTTypeByString(string typeText)
        {
            return typeText switch
            {
                "int" => new TInt(),
                "float" => new TFloat(),
                "string" => new TString(),
                "bool" => new TBool(),
                // "Vector2" => typeof(Vector2),
                // "Vector3" => typeof(Vector3),
                //
                // "List<int>" => typeof(List<int>),
                // "List<float>" => typeof(List<float>),
                // "List<string>" => typeof(List<string>),
                // "List<bool>" => typeof(List<bool>),
                // "List<Vector2>" => typeof(List<Vector2>),
                // "List<Vector3>" => typeof(List<Vector3>),
                //
                // "List<List<int>>" => typeof(List<List<int>>),
                // "List<List<float>>" => typeof(List<List<float>>),
                // "List<List<string>>" => typeof(List<List<string>>),
                // "List<List<bool>>" => typeof(List<List<bool>>),
                // "List<List<Vector2>>" => typeof(List<List<Vector2>>),
                // "List<List<Vector3>>" => typeof(List<List<Vector3>>),
                //
                // "enum" => typeof(Enum),
                // "DateTime" => typeof(DateTime),
                // _ => GetType(typeText)
            };
        }
        
        internal static Type GetTypeByString(string typeText)
        {
            return typeText switch
            {
                "int" => typeof(int),
                "float" => typeof(float),
                "string" => typeof(string),
                "bool" => typeof(bool),
                "Vector2" => typeof(Vector2),
                "Vector3" => typeof(Vector3),
                
                "List<int>" => typeof(List<int>),
                "List<float>" => typeof(List<float>),
                "List<string>" => typeof(List<string>),
                "List<bool>" => typeof(List<bool>),
                "List<Vector2>" => typeof(List<Vector2>),
                "List<Vector3>" => typeof(List<Vector3>),
                
                "List<List<int>>" => typeof(List<List<int>>),
                "List<List<float>>" => typeof(List<List<float>>),
                "List<List<string>>" => typeof(List<List<string>>),
                "List<List<bool>>" => typeof(List<List<bool>>),
                "List<List<Vector2>>" => typeof(List<List<Vector2>>),
                "List<List<Vector3>>" => typeof(List<List<Vector3>>),
                
                "enum" => typeof(Enum),
                "DateTime" => typeof(DateTime),
                _ => GetType(typeText)
            };
        }
        
        /// <summary>
        /// 参考：https://learn.microsoft.com/zh-cn/dotnet/api/system.type.gettype?view=net-8.0
        /// </summary>
        /// <param name="typeText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        internal static Type GetType(string typeText)
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
    }
}