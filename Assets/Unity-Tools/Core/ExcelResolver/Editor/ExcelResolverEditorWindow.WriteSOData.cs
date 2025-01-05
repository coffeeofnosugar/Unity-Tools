﻿using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private Dictionary<ExcelWorksheet, ClassCodeData> classCodeDataDict;
        
        private void WriteSOData()
        {
            foreach (var data in classCodeDataDict)
            {
                var worksheet = data.Key;
                var classCodeData = data.Value;
                
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
            
                    for (int col = 2; col < classCodeData.fields.Keys.Max(); col++)
                    {
                        var cell = worksheet.Cells[row, col];
                        if (string.IsNullOrEmpty(cell.Text)) continue;
                        
                        object convertedValue = ExcelResolverUtil.ConvertCellValue(cell, classCodeData.fields[col].type, classCodeData.className);
                    }
                }
            }
        }
    }
}