using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal static partial class ExcelResolverUtil
    {
        /// <summary>
        /// 更具类型字符串获取 TType
        /// </summary>
        /// <param name="typeText"></param>
        /// <returns></returns>
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
                _ => null,
            };
        }
    }
}