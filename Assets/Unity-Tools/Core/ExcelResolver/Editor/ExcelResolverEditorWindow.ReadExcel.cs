using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void ReadExcel(bool needWrite = true)
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
                
                classCodeData.fields = GetFieldData(worksheet, classCodeData);
                classCodeData.tableType = CheckTableType(worksheet, classCodeData);

                if (needWrite)
                {
                    WriteDataCode(classCodeData);
                    WriteSOCode(classCodeData);
                }
                classCodeDataDict.Add(worksheet, classCodeData);
                // WriteSOData(worksheet, classCodeData);
            }
            
            AssetDatabase.Refresh();
            if (EditorApplication.isCompiling)
            {
                CompilationPipeline.compilationFinished += CompilationFinished;
            }
            else
            {
                WriteSOData();
            }
        }

        private bool isCompilationFinished;
        private void CompilationFinished(object obj)
        {
            CompilationPipeline.compilationFinished -= CompilationFinished;
            Debug.Log("编译完成");
            isCompilationFinished = true;
        }

        private void Update()
        {
            if (isCompilationFinished && System.AppDomain.CurrentDomain.GetAssemblies()
                    .Any(a => a.GetName().Name == "Assembly-CSharp"))
            {
                isCompilationFinished = false;
                Debug.Log("Assembly-CSharp加载完成，开始写入SO数据");
                WriteSOData();
            }
        }

        private static TableType CheckTableType(ExcelWorksheet worksheet, ClassCodeData classCodeData)
        {
            var tableType = TableType.SingleKeyTable;

            string config = worksheet.Cells[1, 1].Text;
            
            if (config.Contains("SingleKeyTable"))
            {
                tableType = TableType.SingleKeyTable;
                var configs = config.Split("|");
                Assert.IsTrue(configs.Length >= 2, $"'{classCodeData.className}'配置错误，SingleKeyTable只能有一个主键");
                var key = configs[1];
                classCodeData.keyField = classCodeData.fields.Where(f => f.Value.varName == key).Select(p => p.Value).ToArray();
            }
            else if (config.Contains("UnionMultiKeyTable"))
            {
                tableType = TableType.UnionMultiKeyTable;
            }
            else if (config.Contains("MultiKeyTable"))
            {
                tableType = TableType.MultiKeyTable;
            }
            else if (config.Contains("NotKetTable"))
            {
                tableType = TableType.NotKetTable;
            }
            else if (config.Contains("ColumnTable"))
            {
                tableType = TableType.ColumnTable;
            }
            else
            {
                Debug.LogError("配置错误");
            }
            
            return tableType;
        }

        private Dictionary<int, FieldData> GetFieldData(ExcelWorksheet worksheet, ClassCodeData classCodeData)
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
            
            // 判断是否有重复的varName
            foreach (var fieldData in fieldDatas.Values)
            {
                if (fieldDatas.Values.Count(f => f.varName == fieldData.varName) > 1)
                {
                    throw new Exception($"'{classCodeData.className}'拥有相同的字段: {fieldData.varName}");
                }
            }

            return fieldDatas;
        }
    }
}