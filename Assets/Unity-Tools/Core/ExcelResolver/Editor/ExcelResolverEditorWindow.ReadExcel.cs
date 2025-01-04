using System.IO;
using System.Linq;
using OfficeOpenXml;
using UnityEngine;

namespace Tools.ExcelResolver.Editor
{
    public sealed partial class ExcelResolverEditorWindow
    {
        private void ReadExcel()
        {
            // 获取Excel文件
            config.MakeSureDirectory();
            var excelFiles = new DirectoryInfo(config.ExcelPathRoot).GetFiles("*.xlsx").Where(f => !f.Name.StartsWith("~$"));
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
                var first = worksheet.Cells[1, 1];
                Debug.Log(first.Value.ToString());
            }
        }
    }
}