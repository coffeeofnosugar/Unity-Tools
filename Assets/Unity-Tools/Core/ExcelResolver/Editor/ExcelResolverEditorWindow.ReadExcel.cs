using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void ReadExcel()
        {
            // 获取Excel文件
            excelResolverConfig.MakeSureDirectory();
            var excelFiles = new DirectoryInfo(excelResolverConfig.ExcelPathRoot).GetFiles("*.xlsx")
                .Where(f => !f.Name.StartsWith("~$"));
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

                var fieldDatas = GetFieldData(worksheet);
                var tableType = CheckTableType(worksheet, out var keyIndex);

                var classCodeData = new ClassCodeData(tableType, excelFile.Name[..^5], fieldDatas, keyIndex);
                
                WriteDataCode(classCodeData);
                WriteSOCode(classCodeData);
                CreateSO(classCodeData);
            }
            
            // AssetDatabase.Refresh();
        }
        
        private TableType CheckTableType(ExcelWorksheet worksheet, out int[] keyIndex)
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
                Assert.IsTrue(configs.Length >= 2, "SingleKeyTable配置错误");
                var key = configs[1];
                var index = getKeyIndex(key);
                Assert.IsTrue(index != -1, "SingleKeyTable配置错误");
                keyIndex = new[] { index };
            }
            else if (config.Contains("UnionMultiKeyTable"))
            {
                type = TableType.UnionMultiKeyTable;
                var configs = config.Split("|");
                Assert.IsTrue(configs.Length >= 2, "UnionMultiKeyTable配置错误");
                var keys = configs[1].Split(",");
                keyIndex = new int[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    var index = getKeyIndex(keys[i]);
                    Assert.IsTrue(index != -1, "UnionMultiKeyTable配置错误");
                    keyIndex[i] = index;
                }
            }
            else if (config.Contains("MultiKeyTable"))
            {
                type = TableType.MultiKeyTable;
                var configs = config.Split("|");
                Assert.IsTrue(configs.Length >= 2, "UnionMultiKeyTable配置错误");
                var keys = configs[1].Split(",");
                keyIndex = new int[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    var index = getKeyIndex(keys[i]);
                    Assert.IsTrue(index != -1, "UnionMultiKeyTable配置错误");
                    keyIndex[i] = index;
                }
            }
            else if (config.Contains("NotKetTable"))
            {
                type = TableType.NotKetTable;
            }
            else if (config.Contains("ColumnTable"))
            {
                type = TableType.ColumnTable;
            }
            else
            {
                Debug.LogError("配置错误");
            }

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

        private FieldData[] GetFieldData(ExcelWorksheet worksheet)
        {
            List<FieldData> fieldDatas = new List<FieldData>();
            var endColumn = worksheet.Dimension.End.Column;
            for (int col = 2; col <= endColumn; col++)
            {
                FieldData fieldData = new FieldData
                {
                    colIndex = col,
                    varName = worksheet.Cells[2, col].Text,
                    typeString = worksheet.Cells[3, col].Text,
                    type = TypeUtil.GetTypeByString(worksheet.Cells[3, col].Text),
                    info = worksheet.Cells[4, col].Text,
                    description = worksheet.Cells[5, col].Text,
                    path = worksheet.Cells[6, col].Text,
                };
                fieldDatas.Add(fieldData);
            }

            return fieldDatas.ToArray();
        }
    }
}