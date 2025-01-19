using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Tools.Editor
{
    /// <summary>
    /// 目录操作的工具类。
    /// </summary>
    public static class DirectoryUtil
    {
        /// <summary>
        /// 确保指定的目录存在。如果不存在，则创建该目录。
        /// </summary>
        /// <param name="path">要检查或创建的目录路径。</param>
        public static void MakeSureDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 获取 "Assets/" 文件夹中所有目录的文件路径，排除某些目录。
        /// </summary>
        /// <returns>文件路径的可枚举集合。</returns>
        public static IEnumerable<string> GetFilePaths()
        {
            var dirs = Directory.GetDirectories("Assets/", "*", SearchOption.AllDirectories);
            var filterList = dirs.Where(file =>
                !file.Contains(".git") &&
                !file.Contains("Plugins") &&
                !file.Contains("Scenes")
            ).Select(file => file.Replace('\\', '/')).ToHashSet();
            return filterList;
        }

        /// <summary>
        /// 从指定路径中提取文件或目录的名称。
        /// </summary>
        /// <param name="path">要提取名称的路径。</param>
        /// <returns>提取的名称。</returns>
        public static string ExtractName(string path)
        {
            var startIndex1 = path.LastIndexOf("/",  StringComparison.Ordinal) + 1;
            var startIndex2 = path.LastIndexOf("\\", StringComparison.Ordinal) + 1;
            var startIndex  = Mathf.Max(startIndex1, startIndex2);
            var endIndex    = path.LastIndexOf(".", StringComparison.Ordinal);
            return endIndex == -1 ? path[startIndex..] : path[startIndex..endIndex];
        }

        /// <summary>
        /// 从指定路径中提取文件夹路径。
        /// </summary>
        /// <param name="path">要提取文件夹路径的路径。</param>
        /// <returns>提取的文件夹路径。</returns>
        public static string ExtractFolder(string path)
        {
            var endIndex1 = path.LastIndexOf("/",  StringComparison.Ordinal);
            var endIndex2 = path.LastIndexOf("\\", StringComparison.Ordinal);
            var endIndex  = Mathf.Max(endIndex1, endIndex2);
            return path[..endIndex];
        }
        
        /// <summary>
        /// 删除目录下所有子目录和文件。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }
            Directory.Delete(path, true);
            return true;
        }
    }
}