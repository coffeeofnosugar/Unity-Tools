using System;
using System.IO;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void WriteSOData(ExcelWorksheet worksheet, ClassCodeData classCodeData)
        {
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
            
            for (int row = 7; row < worksheet.Dimension.End.Row; row++)
            {
                // 跳过注释行
                if (worksheet.Cells[row, 1].Text == "##") continue;
                
                ScriptableObject instance = ScriptableObject.CreateInstance(soType);
            
                for (int col = 2; col < worksheet.Dimension.End.Column; col++)
                {
                    var cell = worksheet.Cells[row, col];
                    // object convertedValue = ExcelResolverUtil.ConvertCellValue(cell, classCodeData.fields[col], classCodeData.fieldNames[col - 2], classCodeData.className);
                }
            }
        }
    }
}