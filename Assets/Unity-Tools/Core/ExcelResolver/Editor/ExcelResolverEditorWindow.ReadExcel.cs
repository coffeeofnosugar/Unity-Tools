using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private enum TableType
        {
            SingleKeyTable,         // 单主键表
            UnionMultiKeyTable,     // 多主键表（联合索引）
            MultiKeyTable,          // 多主键表（独立索引）
            NotKetTable,            // 无主键表
            ColumnTable,            // 纵表
        }


        private void ReadExcel()
        {
            // 获取Excel文件
            excelResolverConfig.MakeSureDirectory();
            var excelFiles = new DirectoryInfo(excelResolverConfig.ExcelPathRoot).GetFiles("*.xlsx").Where(f => !f.Name.StartsWith("~$"));
            foreach (var excelFile in excelFiles)
            {
                using FileStream stream = File.Open(excelFile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using ExcelPackage package = new ExcelPackage(stream);
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                if (null == worksheet)
                {
                    Debug.LogError($"Excel:{excelFile.Name} don't have Sheet1 ！！");
                    continue;
                }
                ExcelRange first = worksheet.Cells[1, 1];

                switch (first)
                {
                    
                }
            }
        }


        private TableType CheckTableType(ExcelRange first)
        {
            string tableConfig = first.Value.ToString();
            
            
            return TableType.SingleKeyTable;
        }
    }
}