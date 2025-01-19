using System;
using System.Collections.Generic;
using System.Reflection;
using OfficeOpenXml;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        [FoldoutGroup("Hide Setting")]
        [ShowInInspector] private Dictionary<ExcelWorksheet, ClassCodeData> classCodeDataDict;
        
        private void WriteSOData()
        {
            if (classCodeDataDict == null)
            {
                ReadExcel(false);
            }
            
            foreach (var classCodeDataDictPair in classCodeDataDict)
            {
                var worksheet = classCodeDataDictPair.Key;
                var classCodeData = classCodeDataDictPair.Value;
                
                Type soType = ExcelResolverUtil.GetOrCacheTypeByName(classCodeData.className);
            
                if (soType == null)
                {
                    Debug.LogError($"Class '{classCodeData.className}SO' not found. Please generate classes first (or check namespace).");
                    return;
                }
            
                // string fullPath = $"{excelResolverConfig.SOPathRoot}/{classCodeData.className}SO.asset";
                //
                // if (File.Exists(fullPath))
                // {
                //     Debug.Log("已存在SO文件，无需创建");
                //     
                // }
                // else
                // {
                //     instance = ScriptableObject.CreateInstance(soType);
                //     AssetDatabase.CreateAsset(instance, $"{excelResolverConfig.SOPathRoot}/{classCodeData.className}SO.asset");
                //     AssetDatabase.SaveAssets();
                // }
            
                for (int row = 7; row <= worksheet.Dimension.End.Row; row++)
                {
                    // 跳过注释行
                    if (worksheet.Cells[row, 1].Text == "##") continue;
                
                    ScriptableObject instance = ScriptableObject.CreateInstance(soType);

                    foreach (var pair in classCodeData.fields)
                    {
                        var col = pair.Key;
                        var fieldData = pair.Value;
                        
                        var cell = worksheet.Cells[row, col];
                        if (string.IsNullOrEmpty(cell.Text)) continue;

                        object convertedValue = ExcelResolverUtil.ConvertCellValue(cell, fieldData.type, classCodeData.className);
                        FieldInfo fieldInfo = soType.GetField(fieldData.varName);
                        if (fieldInfo == null) throw new Exception($"目标类中不存在字段：{fieldData.varName}");
                        fieldInfo.SetValue(instance, convertedValue);
                    }
                    AssetDatabase.CreateAsset(instance, $"{excelResolverConfig.SOPathRoot}/{classCodeData.className}_{row - 6}.asset");
                }
                AssetDatabase.SaveAssets();
            }
        }
    }
}