using System.Collections.Generic;

namespace Tools.ExcelResolver.Editor
{
    internal class ClassCodeData
    {
        public readonly TableType tableType;
        public readonly string className;
        public readonly Dictionary<int, FieldData> fields = new();
        // public readonly FieldData[] fields;
        public readonly int[] keyIndex;
        // public readonly FieldData[] keyField;

        public ClassCodeData(TableType tableType, string className, Dictionary<int, FieldData> fields, int[] keyIndex)
        {
            this.tableType = tableType;
            this.className = $"{char.ToUpper(className[0])}{className.Substring(1)}";
            this.fields = fields;
            this.keyIndex = keyIndex;
            
            // keyField = new FieldData[keyIndex.Length];
            // for (int i = 0; i < keyIndex.Length; i++)
            // {
            //     keyField[i] = fields.First(f => f.colIndex == keyIndex[i]);
            // }
        }
    }
}