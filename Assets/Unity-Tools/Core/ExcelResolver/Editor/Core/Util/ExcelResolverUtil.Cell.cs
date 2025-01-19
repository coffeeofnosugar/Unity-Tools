using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    internal static partial class ExcelResolverUtil
    {
        /// <summary>
        /// 读取指定行的数据
        /// </summary>
        internal static List<string> ReadRow(ExcelWorksheet worksheet, int row, int colCount, string rowType)
        {
            List<string> values = new(colCount);
            for (int col = 1; col <= colCount; col++)
            {
                string value = worksheet.Cells[row, col].Text?.Trim();
                if (rowType == "headers" && string.IsNullOrEmpty(value))
                {
                    Debug.LogWarning(
                        $"Empty header found in worksheet '{worksheet.Name}', column {col}. Skipping this column.");
                    values.Add(string.Empty);
                    continue;
                }

                if (rowType == "types" && string.IsNullOrEmpty(value))
                {
                    value = "string";
                    Debug.LogWarning(
                        $"Empty type found for column '{worksheet.Cells[1, col].Text}' in worksheet '{worksheet.Name}'. Defaulting to 'string'.");
                }

                values.Add(value);
            }
            return values;
        }

        /// <summary>
        /// 将TType转换为C#对象
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="type"></param>
        /// <param name="className"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static object ConvertCellValue<T>(ExcelRange cell, T type, string className)
            where T : TType
        {
            var result = type.TryParseFrom(cell.Text);
            if (result != null)
            {
                return result;
            }
            
            throw new Exception($"单元格转换失败   FullAddress: {cell.FullAddress}   Text: {cell.Text}   className: {className}");
        }
        
        /// <summary>
        /// 通用的单元格 -> C# 对象转换
        /// </summary>
        internal static object ConvertCellValue(ExcelRange cell, string type, string header, string className)
        {
            try
            {
                string cellValue = cell.Value?.ToString()?.Trim();

                if (type.StartsWith("list<", StringComparison.OrdinalIgnoreCase) && type.EndsWith(">"))
                {
                    return ConvertToList(cellValue, type, header, className);
                }
                else if (TypeConverters.TryGetValue(type, out var converter))
                {
                    return converter(cellValue);
                }
                else
                {
                    Debug.LogError($"Unsupported type '{type}' for field '{header}' in class '{className}'.");
                    return GetDefaultValue(type);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error converting value for '{header}' in '{className}': {ex.Message}");
                return GetDefaultValue(type);
            }
        }

        /// <summary>
        /// 返回类型的默认值 (包含 List<...> 场景)
        /// </summary>
        internal static object GetDefaultValue(string type)
        {
            if (type.StartsWith("list<", StringComparison.OrdinalIgnoreCase) && type.EndsWith(">"))
            {
                var insideType = type.Substring(5, type.Length - 6).Trim();
                return insideType.ToLower() switch
                {
                    "int" or "integer" => new List<int>(),
                    "float" => new List<float>(),
                    "double" => new List<double>(),
                    "bool" or "boolean" => new List<bool>(),
                    "string" => new List<string>(),
                    _ => null
                };
            }

            return type.ToLower() switch
            {
                "int" or "integer" => 0,
                "float" => 0f,
                "double" => 0.0,
                "bool" or "boolean" => false,
                "string" => string.Empty,
                _ => null,
            };
        }

        /// <summary>
        /// 转换逗号分隔的字符串到 List<...> (List<int>, List<string>, ...)
        /// </summary>
        internal static object ConvertToList(string cellValue, string type, string header, string className)
        {
            var insideType = type.Substring(5, type.Length - 6).Trim();
            var splitted = string.IsNullOrEmpty(cellValue)
                ? Array.Empty<string>() 
                : cellValue.Split(',');

            return insideType.ToLower() switch
            {
                "int" or "integer" => splitted.Select(s => {
                    int.TryParse(s.Trim(), out var parsed); return parsed;
                }).ToList(),
                "float" => splitted.Select(s => {
                    float.TryParse(s.Trim(), out var parsed); return parsed;
                }).ToList(),
                "double" => splitted.Select(s => {
                    double.TryParse(s.Trim(), out var parsed); return parsed;
                }).ToList(),
                "bool" or "boolean" => splitted.Select(s => {
                    return bool.TryParse(s.Trim(), out var parsed) ? parsed : (s.Trim() == "1" || s.Trim().Equals("true", StringComparison.OrdinalIgnoreCase));
                }).ToList(),
                "string" => splitted.Select(s => s.Trim()).ToList(),
                _ => throw new Exception($"Unsupported list element type '{insideType}' for field '{header}' in class '{className}'.")
            };
        }
        
        internal static readonly Dictionary<string, Func<string, object>> TypeConverters = new(StringComparer.OrdinalIgnoreCase)
        {
            { "int", value => int.TryParse(value, out var intValue) ? intValue : 0 },
            { "float", value => float.TryParse(value, out var floatValue) ? floatValue : 0f },
            { "double", value => double.TryParse(value, out var doubleValue) ? doubleValue : 0.0 },
            { "bool", value => bool.TryParse(value, out var boolValue)
                ? boolValue
                : (value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase)) },
            { "string", value => value ?? string.Empty }
        };
    }
}