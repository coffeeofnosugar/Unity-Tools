using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tools.EventBus
{
    /// <summary>
    /// 在预定义程序集内查找实现了某个接口的所有类型
    /// <see href="https://docs.unity3d.com/2023.3/Documentation/Manual/ScriptCompileOrderFolders.html">visit Unity Documentation</see>
    /// </summary>
    public static class PredefinedAssemblyUtil
    {
        /// <summary> 四种不同类型的程序集 </summary>    
        enum AssemblyType
        {
            AssemblyCSharp,
            AssemblyCSharpEditor,
            AssemblyCSharpEditorFirstPass,
            AssemblyCSharpFirstPass
        }

        /// <summary> 将字符串的程序集名称转换成<see cref="AssemblyType"/>枚举 </summary>
        static AssemblyType? GetAssemblyType(string assemblyName)
        {
            return assemblyName switch
            {
                "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
                "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
                "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
                "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
                _ => null
            };
        }

        /// <summary>
        /// 遍历一个程序集中所有的类型，筛选出实现了某个接口的类型（排除接口类型本身），并添加到指定集合中
        /// </summary>
        /// <param name="assemblyTypes">某个程序集中所有类型</param>
        /// <param name="interfaceType">指定要筛选的接口类型</param>
        /// <param name="results">筛选出的类型会添加到该集合中</param>
        static void AddTypesFromAssembly(Type[] assemblyTypes, Type interfaceType, ICollection<Type> results)
        {
            if (assemblyTypes == null) return;
            foreach (var type in assemblyTypes)
            {
                if (type != interfaceType && interfaceType.IsAssignableFrom(type))
                    results.Add(type);
            }
        }
        
        /// <summary> AppDomain 中实现了指定接口的所有类型。 </summary>
        public static List<Type> GetTypes(Type interfaceType)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
            List<Type> types = new List<Type>();
            foreach (var t in assemblies)
            {
                AssemblyType? assemblyType = GetAssemblyType(t.GetName().Name);
                if (assemblyType != null)
                    assemblyTypes.Add((AssemblyType)assemblyType, t.GetTypes());
            }
            
            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharp, out var assemblyCSharpTypes);
            AddTypesFromAssembly(assemblyCSharpTypes, interfaceType, types);

            assemblyTypes.TryGetValue(AssemblyType.AssemblyCSharpFirstPass, out var assemblyCSharpFirstPassTypes);
            AddTypesFromAssembly(assemblyCSharpFirstPassTypes, interfaceType, types);
            
            return types;
        }
    }
}