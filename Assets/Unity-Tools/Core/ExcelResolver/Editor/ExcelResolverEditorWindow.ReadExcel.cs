using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void ReadExcel()
        {
            classCodeDataDict = new Dictionary<ExcelWorksheet, ClassCodeData>();
            
            // 获取Excel文件
            excelResolverConfig.MakeSureDirectory();
            var excelFiles = new DirectoryInfo(excelResolverConfig.ExcelPathRoot).GetFiles("*.xlsx").Where(f => !f.Name.StartsWith("~$"));
            foreach (var excelFile in excelFiles)
            {
                using FileStream stream = File.Open(excelFile.FullName, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite);
                using ExcelPackage package = new ExcelPackage(stream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                if (null == worksheet)
                {
                    Debug.LogError($"Excel:{excelFile.Name} don't have Sheet1 ！！");
                    continue;
                }
                
                var classCodeData = new ClassCodeData(excelFile.Name[..^5]);
                
                var fieldDatas = GetFieldData(worksheet);
                classCodeData.fields = fieldDatas;
                var tableType = CheckTableType(worksheet, classCodeData.className, out var keyIndex);
                classCodeData.tableType = tableType;
                classCodeData.keyIndex = keyIndex;

                
                WriteDataCode(classCodeData);
                WriteSOCode(classCodeData);
                classCodeDataDict.Add(worksheet, classCodeData);
                // WriteSOData(worksheet, classCodeData);
            }
            AssetDatabase.Refresh();
        }
        
        private TableType CheckTableType(ExcelWorksheet worksheet, string className, out int[] keyIndex)
        {
            var startColumn = worksheet.Dimension.Start.Column; // 起始列
            var endColumn = worksheet.Dimension.End.Column; // 结束列

            string config = worksheet.Cells[1, 1].Text;
            
            var type = TableType.SingleKeyTable;
            keyIndex = null;
            
            if (config.Contains("SingleKeyTable"))
            {
                type = TableType.SingleKeyTable;
                var configs = config.Split("|");
                Assert.IsTrue(configs.Length >= 2, $"'{className}'配置错误");
                var key = configs[1];
                var index = getKeyIndex(key);
                Assert.IsTrue(index != -1, $"'{className}'配置错误");
                keyIndex = new[] { index };
            }
            // else if (config.Contains("UnionMultiKeyTable"))
            // {
            //     type = TableType.UnionMultiKeyTable;
            //     var configs = config.Split("|");
            //     Assert.IsTrue(configs.Length >= 2, "UnionMultiKeyTable配置错误");
            //     var keys = configs[1].Split(",");
            //     keyIndex = new int[keys.Length];
            //     for (int i = 0; i < keys.Length; i++)
            //     {
            //         var index = getKeyIndex(keys[i]);
            //         Assert.IsTrue(index != -1, "UnionMultiKeyTable配置错误");
            //         keyIndex[i] = index;
            //     }
            // }
            // else if (config.Contains("MultiKeyTable"))
            // {
            //     type = TableType.MultiKeyTable;
            //     var configs = config.Split("|");
            //     Assert.IsTrue(configs.Length >= 2, "UnionMultiKeyTable配置错误");
            //     var keys = configs[1].Split(",");
            //     keyIndex = new int[keys.Length];
            //     for (int i = 0; i < keys.Length; i++)
            //     {
            //         var index = getKeyIndex(keys[i]);
            //         Assert.IsTrue(index != -1, "UnionMultiKeyTable配置错误");
            //         keyIndex[i] = index;
            //     }
            // }
            // else if (config.Contains("NotKetTable"))
            // {
            //     type = TableType.NotKetTable;
            // }
            // else if (config.Contains("ColumnTable"))
            // {
            //     type = TableType.ColumnTable;
            // }
            // else
            // {
            //     Debug.LogError("配置错误");
            // }

            return type;

            int getKeyIndex(string key)
            {
                int keyIndex = -1;
                for (int col = startColumn; col <= endColumn; col++)
                {
                    var cellValue = worksheet.Cells[2, col].Text; // 获取第二行第 col 列的文本值
                    if (string.Equals(cellValue, key, StringComparison.OrdinalIgnoreCase)) // 忽略大小写比较
                    {
                        keyIndex = col;
                        break;
                    }
                }

                return keyIndex;
            }
        }

        private Dictionary<int, FieldData> GetFieldData(ExcelWorksheet worksheet)
        {
            var fieldDatas = new Dictionary<int, FieldData>();
            
            for (int col = 2; col <= worksheet.Dimension.End.Column; col++)
            {
                var cellText = worksheet.Cells[2, col].Text;
                if (string.IsNullOrEmpty(cellText) || cellText == "##") continue;
                
                FieldData fieldData = new FieldData
                {
                    colIndex = col,
                    varName = cellText,
                    typeString = worksheet.Cells[3, col].Text,
                    type = ExcelResolverUtil.GetTTypeByString(worksheet.Cells[3, col].Text),
                    info = worksheet.Cells[4, col].Text,
                    description = worksheet.Cells[5, col].Text,
                    path = worksheet.Cells[6, col].Text,
                };
                fieldDatas.Add(col, fieldData);
            }

            return fieldDatas;
        }
    }
}