using System.Collections.Generic;

namespace Tools.ExcelResolver.Editor
{
    internal class ClassCodeData
    {
        public TableType tableType;
        public string className;
        public Dictionary<int, FieldData> fields = new();
        // public FieldData[] fields;
        // public int[] keyIndex;
        public FieldData[] keyField;

        public ClassCodeData(string className)
        {
            this.className = $"{char.ToUpper(className[0])}{className.Substring(1)}";
            
            // keyField = new FieldData[keyIndex.Length];
            // for (int i = 0; i < keyIndex.Length; i++)
            // {
            //     keyField[i] = fields.First(f => f.colIndex == keyIndex[i]);
            // }
        }
    }
}