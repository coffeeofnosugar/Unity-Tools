using System;
using System.Linq;
using System.Reflection;

namespace Tools.ExcelResolver.Editor
{
    internal static partial class ExcelResolverUtil
    {
        private static TType[] _allTTypes;
        
        internal static TType[] GetAllTTypes()
        {
            return Assembly.GetAssembly(typeof(TType))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(TType)) && !t.IsAbstract)
                .Select(t => Activator.CreateInstance(t) as TType)
                .ToArray();
        }
        
        /// <summary>
        /// 更具类型字符串获取 TType
        /// </summary>
        /// <param name="typeText"></param>
        /// <returns></returns>
        internal static TType GetTTypeByString(string typeText)
        {
            
            typeText = typeText.ToLower();
            _allTTypes ??= GetAllTTypes();
            
            foreach (var tType in _allTTypes)
            {
                if (tType.String2TType(typeText))
                {
                    return tType;
                }
            }

            throw new Exception($"未找到类型 {typeText}");
            
            return typeText switch
            {
                "int" => new TInt(),
                "float" => new TFloat(),
                "string" => new TString(),
                "bool" => new TBool(),
                "vector2" => new TVector2(),
                "vector3" => new TVector3(),
                
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